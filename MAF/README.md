# Engenharia_de_Software
Repositório dedicado ao projeto de Engenharia de Software.

# Estrutura do Projeto

Este projeto segue a **Arquitetura Hexagonal**, também conhecida como Arquitetura de Portas e Adaptadores. O objetivo principal é manter o **núcleo** da aplicação (a lógica de negócio) completamente isolado e independente de tecnologias externas, como bancos de dados ou APIs.

A comunicação entre o núcleo e o mundo exterior é feita exclusivamente através de **portas** (interfaces), que são implementadas por **adaptadores**.

---

## Componentes da Estrutura

### **Core**

Este é o **núcleo** da sua aplicação. Ele contém a lógica de negócio pura, as regras e o domínio do problema. Ele é agnóstico em relação a qualquer tecnologia.

-   **Application**: Contém a lógica de orquestração e os casos de uso. É aqui que os serviços de aplicação (que implementam as portas de entrada e possui um objeto da porta de saida) coordenam as operações do sistema.

-   **Domain**: Contém as entidades de negócio (os modelos de dados(Classes))

### **Ports (Portas)**

As portas são as **interfaces** que definem como o núcleo se comunica. Elas estabelecem o contrato de comunicação.

-   **Input**: Interfaces que a lógica de negócio **expõe** para ser usada por atores externos. É o contrato que os adaptadores de entrada devem seguir.
-   **Output**: Interfaces que a lógica de negócio **precisa** para se comunicar com serviços externos. É o contrato que os adaptadores de saída devem seguir.

### **Adapters (Adaptadores)**

Os adaptadores são as API's externas que usam as interfaces das portas para construirem sua logica.

-   **Input**: Implementam as portas de entrada. Um exemplo é uma **API REST**, que recebe requisições HTTP e as traduz para chamadas à lógica de negócio no núcleo.
-   **Output**: Implementam as portas de saída. Um exemplo é um **repositório de banco de dados**, que recebe uma chamada do núcleo e a traduz para operações de persistência, como um Firebase.

### **Front-end**

Parte que será responsável por gerar as telas HTML com CSS e Javascript e a logica de cominicação com a API do Maps.


# Comandos dotnet CLI

### Criar uma nova biblioteca de classes (Class Library) e adicionar o .csproj novo a partir da raiz:

```bash
dotnet new classlib -n NomeDoProjeto -o Local/do/Projeto
```

### Adicionar no arquivo .sln
```bash
dotnet sln add Caminho/do/*.csproj
```
### Remover do arquivo .sln
```bash
dotnet sln remove Caminho/do/*.csproj
```

### Adicionar uma referência entre projetos:

como são vários projetos(classlib) toda vez que adicionar uma referência lembre-se de dar build no projeto em questão.

```bash
#caso estaja dentro do projeto que recebera a referencia
dotnet add reference Caminho/Do/Projeto/Referencia
dotnet build

#comando de qualquer lugar
dotnet add Caminho/Do/Projeto/Destino reference Caminho/Do/Projeto/Referencia
dotnet build Caminho/Do/Projeto/Destino
```
### Adiconar um package

```bash
cd caminho/do/diretorio/projeto.csproj

dotnet add package <PACKAGE_NAME>
```

### Mostrar todos os projetos relacionados ao MAF.sln:

```bash
#executado na raiz do projeto

dotnet sln list
```

### Compilar o projeto:

```bash
#executado na raiz do projeto

dotnet build
```

### Executar o projeto localizado em 'Adapters/Input/Web' caso execute a partir da raiz do projeto:

```bash
dotnet run --project Adapters/Input/Web
```
### Execute o seguinte comando para iniciar o aplicativo no perfil https a partir da raiz do projeto:
Outra opção se preferir executar com o perfil HTTPS.


```bash
dotnet run --project Adapters/Input/Web --launch-profile https 
```

### Como buildar e subir o código no Google Cloud Run

É necessário ter o Google Cloud SDK e o Docker Desktop

Dentro do SDK rode o comando:

gcloud config set project arvores-e8c4c

Após isso rode:

gcloud auth configure-docker us-central1-docker.pkg.dev

E por fim rode:



para saber mais caso deu um erro: [Trust the HTTPS](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio-code#:~:text=Trust%20the%20HTTPS%20development%20certificate%20by%20running%20the%20following%20command%3A)