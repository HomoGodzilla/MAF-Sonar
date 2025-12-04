using MAF.Ports.Input;
using MAF.Ports.Output;
using MAF.Core.Domain;

namespace MAF.Core.Application;


public class UserApplication : IUserInput
{
    private IUserOutput UserOutput { get; }

    public UserApplication(IUserOutput user_output)
    {
        UserOutput = user_output;
    }

    public User RegisterUser(string name, string email, string password)
    {
        /*Exceptions: InvalidEmailException
            PasswordOutOfRangeException
            NameOutOfRangeException
            */
        return UserOutput.RegisterUserDB(new User(name, email, password));
    }

    public User GetUser(string email, string password)//Para uso do admin e fazer Login
    {
        User.ValidEmail(email);//Exception: InvalidEmailException

        return UserOutput.GetUserDB(email, password);// Exception: Alguma exception do DB
    }

    public bool DeleteUser(string email, string password)
    {

        User.ValidEmail(email); //Exception: InvalidEmailException

        return UserOutput.DeleteUserDB(email, password);

    }
    public bool UpdateEmail(string old_email, string password, string new_email)
    {
        User.ValidEmail(new_email);//Exception: InvalidEmailException

        return UserOutput.UpdateEmailDB(old_email, password, new_email);
    }
    public bool UpdateName(string email, string password, string new_name)
    {

        User.ValidName(new_name);//Exception: NameOutOfRangeException

        return UserOutput.UpdateNameDB(email, password, new_name);
    }
    public bool UpdatePW(string email, string old_password, string new_password)
    {
        User.ValidEmail(email);//Exception: InvalidEmailException

        User.ValidPassword(new_password);//Exception: PasswordOutOfRangeException

        return UserOutput.UpdatePwDB(email, new_password);
    }
}

public class TreeApplication : ITreeInput
{
    private ITreeOutput TreeOutput { get; }

    public TreeApplication(ITreeOutput tree_output)
    {

        TreeOutput = tree_output;
    }

    public List<Tree> GetAllTrees()
    {
        return TreeOutput.GetAllTreesDB();
    }

    public Tree RegisterTree(Tree new_tree)
    {
        return TreeOutput.RegisterTreeDB(new_tree);
    }

    public Tree GetTree(Location location)
    {
        return TreeOutput.GetTreeDB(location);
    }
    public Photo GetTreePhoto(Location location)
    {
        return TreeOutput.GetTreePhotoDB(location);
    }

    public Tree[] GetUserTree(string email_user)
    {
        // O Banco de dados tem que retornar um 'new int[0];' se nao fudeo
        return TreeOutput.GetUserTreeDB(email_user);
    }

    public bool DeleteTree(Location location)
    {
        return TreeOutput.DeleteTreeDB(location);
    }
    public bool UpdateNameTree(string new_name, Location location)
    {
        Tree.ValidSizeStrTree(new_name, 0);//Exception: NameOutOfRangeException

        return TreeOutput.UpdateNameTreeDB(new_name, location);
    }

    public bool UpdateSciTree(string new_sci_name, Location location)
    {
        Tree.ValidSizeStrTree(new_sci_name, 2);//Exception: SciNameOutOfRangeException

        return TreeOutput.UpdateSciTreeDB(new_sci_name, location);
    }

    public bool UpdateDescTree(string new_desc, Location location)
    {
        Tree.ValidSizeStrTree(new_desc, 3);//Exception: DescrOutOfRangeException

        return TreeOutput.UpdateDescTreeDB(new_desc, location);
    }

    public bool UpdateFruitTree(string new_fruit, Location location)
    {
        Tree.ValidSizeStrTree(new_fruit, 1);//Exception: FruitNameOutOfRangeException

        return TreeOutput.UpdateFruitTreeDB(new_fruit, location);
    }
}