using MAF.Core.Domain;

namespace MAF.Ports.Input;

public interface IUserInput
{
    User RegisterUser(string name, string email, string password);
    
    User GetUser(string email,string password);
    bool DeleteUser(string email, string password);
    bool UpdateEmail(string old_email, string password, string new_email);
    bool UpdateName(string email, string password, string new_name);
    bool UpdatePW(string email, string old_password, string new_password);
    
}


public interface ITreeInput
{
    Tree RegisterTree(Tree new_tree);
    Tree GetTree(Location location);
    Photo GetTreePhoto(Location location);
    List<Tree> GetAllTrees();
    Tree[] GetUserTree(string email_user);
    bool DeleteTree(Location location);
    bool UpdateNameTree(string new_name, Location location);
    bool UpdateSciTree(string new_sci_name, Location location);
    bool UpdateDescTree(string new_desc, Location location);
    bool UpdateFruitTree(string new_fruit, Location location);
    
}