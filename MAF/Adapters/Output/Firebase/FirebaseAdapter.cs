using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using MAF.Core.Domain;
using MAF.Ports.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MAF.Adapters.Output.Firebase
{
    // Corrigido para implementar a interface correta: ITreeOutput
    public class FirebaseAdapter : ITreeOutput
    {
        private readonly FirestoreDb _db;
        private readonly StorageClient _storageClient;
        private readonly string _bucketName = "arvores-e8c4c.firebasestorage.app"; // <-- IMPORTANTE: Substitua pelo nome do seu bucket do Firebase Storage

        public FirebaseAdapter()
        {
            var credentialPath = "../../../credentials/arvores-e8c4c-firebase-adminsdk-fbsvc-40b5db679e.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            const string projectId = "arvores-e8c4c";

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialPath),
                    ProjectId = projectId,
                });
            }
            _db = FirestoreDb.Create(projectId);

            // Inicializa o cliente do Storage
            _storageClient = StorageClient.Create(GoogleCredential.FromFile(credentialPath));
        }

        // Método para fazer upload da foto para o Firebase Storage
        private async Task<string> UploadPhotoAsync(Photo photo)
        {
            var credentialPath = Path.Combine(AppContext.BaseDirectory, "credentials", "arvores-e8c4c-firebase-adminsdk-fbsvc-40b5db679e.json");
            var googleCredential = GoogleCredential.FromFile(credentialPath);
            var storage = StorageClient.Create(googleCredential);

            using (var memoryStream = new MemoryStream())
            {
                await photo.GetStreamBytes().CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reinicia a posição do stream

                var objectName = $"tree_photos/{photo.GetNameFile()}";

                storage.UploadObject(_bucketName, objectName, photo.GetContentType(), memoryStream);

                // Retorna a URL pública do arquivo
                return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
            }
        }

        public Tree RegisterTreeDB(Tree new_tree)
        {
            try
            {
                // Faz o upload da foto e obtém a URL
                string photoUrl = UploadPhotoAsync(new_tree.GetPhoto()).Result;

                CollectionReference arvoresRef = _db.Collection("arvores-frutiferas");

                var novaArvoreData = new Dictionary<string, object>
                {
                    { "Name", new_tree.GetName() },
                    { "Fruit", new_tree.GetFruit() },
                    { "SciName", new_tree.GetSciName() },
                    { "Description", new_tree.GetDescription() },
                    { "FkEmail", new_tree.GetFkEmail() },
                    { "PhotoUrl", photoUrl }, // Salva a URL da foto
                    { "Location", new GeoPoint(new_tree.GetLocation().GetLatitude(), new_tree.GetLocation().GetLongitude()) },
                    { "CreatedAt", Timestamp.FromDateTime(new_tree.GetCreateAt().ToUniversalTime()) }
                };

                // Usamos a localização como um ID único para facilitar a busca
                string documentId = $"{new_tree.GetLocation().GetLatitude()}_{new_tree.GetLocation().GetLongitude()}";
                arvoresRef.Document(documentId).SetAsync(novaArvoreData).Wait();

                Console.WriteLine("FirebaseAdapter: Árvore adicionada com sucesso!");
                return new_tree;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro no FirebaseAdapter ao registrar árvore: {e.Message}");
                throw;
            }
        }

        public List<Tree> GetAllTreesDB()
        {
            try
            {
                List<Tree> arvores = new List<Tree>();
                CollectionReference arvoresRef = _db.Collection("arvores-frutiferas");
                QuerySnapshot snapshot = arvoresRef.GetSnapshotAsync().Result;

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        Dictionary<string, object> treeData = document.ToDictionary();

                        // Extrai a localização do GeoPoint
                        var locationPoint = (GeoPoint)treeData["Location"];
                        var location = new Location(locationPoint.Latitude, locationPoint.Longitude);

                        var tree = new Tree(
                            treeData.ContainsKey("Name") ? treeData["Name"].ToString() : "",
                            treeData.ContainsKey("Fruit") ? treeData["Fruit"].ToString() : "",
                            treeData.ContainsKey("FkEmail") ? treeData["FkEmail"].ToString() : "",
                            treeData.ContainsKey("SciName") ? treeData["SciName"].ToString() : "",
                            location,
                            treeData.ContainsKey("Description") ? treeData["Description"].ToString() : "",
                            // Para a lista, só precisamos da URL, que estará no DTO
                            new Photo(new MemoryStream(), "image/jpeg", ".jpg")
                        );

                        arvores.Add(tree);
                    }
                }
                return arvores;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro no FirebaseAdapter ao buscar todas as árvores: {e.Message}");
                throw;
            }
        }


        public Tree GetTreeDB(Location location)
        {
            // Implementação para obter uma árvore do Firebase
            // (Esta é uma implementação de exemplo, ajuste conforme necessário)
            string documentId = $"{location.GetLatitude()}_{location.GetLongitude()}";
            DocumentReference docRef = _db.Collection("arvores-frutiferas").Document(documentId);
            DocumentSnapshot snapshot = docRef.GetSnapshotAsync().Result;

            if (snapshot.Exists)
            {
                var treeData = snapshot.ToDictionary();
                // Aqui você precisaria reconstruir o objeto Tree a partir dos dados do Firebase
                // Esta parte é simplificada
                return new Tree(
                    treeData["Name"].ToString()!,
                    treeData["Fruit"].ToString()!,
                    treeData["FkEmail"].ToString()!,
                    treeData["SciName"].ToString()!,
                    location, // já temos a localização
                    treeData["Description"].ToString()!,
                    new Photo(new MemoryStream(), "image/jpeg", ".jpg") // Foto é um placeholder
                );
            }
            throw new KeyNotFoundException("Árvore não encontrada no banco de dados.");
        }

        // --- Implementações dos outros métodos da interface ---

        public Photo GetTreePhotoDB(Location location)
        {
            // Lógica para obter a URL do Firestore e depois baixar a foto do Storage
            throw new NotImplementedException();
        }

        public Tree[] GetUserTreeDB(string email_user)
        {
            // Lógica para buscar todas as árvores de um usuário
            throw new NotImplementedException();
        }

        public bool DeleteTreeDB(Location location)
        {
            try
            {
                string documentId = $"{location.GetLatitude()}_{location.GetLongitude()}";
                DocumentReference docRef = _db.Collection("arvores-frutiferas").Document(documentId);
                docRef.DeleteAsync().Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateNameTreeDB(string new_name, Location location)
        {
            try
            {
                string documentId = $"{location.GetLatitude()}_{location.GetLongitude()}";
                DocumentReference docRef = _db.Collection("arvores-frutiferas").Document(documentId);
                docRef.UpdateAsync("Name", new_name).Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ... Implementar os outros métodos de Update (UpdateSciTreeDB, UpdateDescTreeDB, UpdateFruitTreeDB) de forma similar ...

        public bool UpdateSciTreeDB(string new_sci_name, Location location)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDescTreeDB(string new_desc, Location location)
        {
            throw new NotImplementedException();
        }

        public bool UpdateFruitTreeDB(string new_fruit, Location location)
        {
            throw new NotImplementedException();
        }
    }
}