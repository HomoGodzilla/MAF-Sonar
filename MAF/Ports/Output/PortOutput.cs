namespace MAF.Ports.Output;

using MAF.Core.Domain;

public interface IUserOutput
{
    User RegisterUserDB(User user);
    
    User GetUserDB(string email,string password);
    bool DeleteUserDB(string email, string password);
    bool UpdateEmailDB(string old_email, string password, string new_email);
    bool UpdateNameDB(string email, string password, string new_name);
    bool UpdatePwDB(string email, string new_password);
    
}


public interface ITreeOutput
{
    Tree RegisterTreeDB(Tree new_tree);
    Tree GetTreeDB(Location location);
    Photo GetTreePhotoDB(Location location);
    List<Tree> GetAllTreesDB();
    Tree[] GetUserTreeDB(string email_user);
    bool DeleteTreeDB(Location location);
    bool UpdateNameTreeDB(string new_name, Location location);
    bool UpdateSciTreeDB(string new_sci_name, Location location);
    bool UpdateDescTreeDB(string new_desc, Location location);
    bool UpdateFruitTreeDB(string new_fruit, Location location);
    
}

