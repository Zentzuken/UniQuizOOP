// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

/* TODO
*/
// DEBUGS ------------------------------------------------------------------------------------

//const string connectionString = "Server=localhost; User ID=root; Password=godeater0; Database=Shop0";

// Add a new thingy to Database -------------------------------------------
/*
using (var context = new Shop0DataContext())
{
    var product = new Product { name = "FireArm", stock = 10, price = 699.99, desc = "America Moment" };
    context.Products.Add(product);
    context.SaveChanges();
}
//*/
// -------------------------------------------------------------------------------------


// Create User TODO (Login account function here)=============================
User main_user = new User("Ein", 10000);


// MAIN SHOP ===================================================================================
using (var context = new Shop0DataContext())
{

    // create user cart 
    Transaction user_cart = new Transaction();


    string command = "start";
    Console.WriteLine("\n Welcome to the shop ! --------------------------\n");

    // show user info
    Console.WriteLine(
        "{0,15}\t{1,20}", 
        $"Username : {main_user.name}", $"Balance : {main_user.balance}"
        );
    
    Console.WriteLine("\n");


    // MAIN main online shop command driver and manager ========================
    while (command != "exit") 
    {
        // StartUp message + tutorial
        Console.Write(
            
            "Here are all available commands, simply type them on the terminal : \n" +
            "1. 'exit' to exit the store \n" +
            "2. 'user' to see your user informations \n" +
            "3. 'store' to see all available products \n" +
            "4. 'cart' to see your cart \n" +
            "5. 'add' to add an item to your cart \n" +
            "6. 'remove' to remove an item from your cart \n" +
            "7. 'checkout' to checkout your shopping cart \n"
            );


        // get user input 
        Console.Write("\n Type command : ");
        command = Console.ReadLine();
        Console.Write("\n");


        switch (command)
        {
            // show user information
            case "user":

                Console.WriteLine(
                    "{0,15}\t{1,20}", 
                    $"Username : {main_user.name}", $"Balance : {main_user.balance}"
                    );
                
                Console.WriteLine("\n");
                break;


            // Show products -------------
            case "store":

                // print products
                Console.WriteLine(
                    "{0,15}\t{1,10}\t{2,5}\t{3,20}",
                    "Name", "Price", "Stock", "Description"
                    ); 

                foreach (var product in context.Products)
                {
                    Console.WriteLine(
                        "{0,15}\t{1,10}\t{2,5}\t{3,20}", 
                        $"{product.name}", $"{product.price}", $"{product.stock}", $"{product.desc}"
                        );
                }

                break;
            

            // Show cart -------------------
            case "cart":
    
                // check if cart empty 
                if (!user_cart.cart.Any())
                {   
                    Console.WriteLine("\n Cart is empty \n");
                }

                // Print user cart if not empty
                else 
                {
                    user_cart.print_cart();
                }
                break;
            

            // Add an item to user cart ----
            case "add":

                // get item from user
                Console.Write("Type the item you wish to add to your cart : ");
                string new_item = Console.ReadLine();

                // check if product exists
                foreach (var item in context.Products)
                {
                    
                    if (item.name == new_item) 
                    {

                        // clone the product obj from database
                        Product item_clone = (Product)item.Shallowcopy();

                        // add cloned to cart
                        user_cart.add_product(item_clone);

                        // TODO reduce stock of thhat context.product from database

                        Console.Write("\n The item '" + new_item + "' has been added to your cart \n");
                        break;
                    }
                    else {

                        // item inputed not found
                        Console.Write("\n The item '" + new_item + "' doesn't exists \n");
                        break;
                    }

                }
                break;


            // remove item from cart -------
            case "remove":

                // get item from user
                Console.Write("Type the item you wish to remove from your cart : ");
                string no_item = Console.ReadLine();

                if (no_item != null)
                {
                    user_cart.remove_product(no_item);
                }
                else
                {
                    Console.Write("no item inputed \n");
                }

                break;

            
            // Buy all items in ze cart ----
            case "checkout":

                // print cart items
                Console.WriteLine("Your current cart items : \n");
                user_cart.print_cart();

                // print cart price
                Console.WriteLine("Your cart total : " + user_cart.calculate_cart() + "\n");

                // ask to confirm checkout
                Console.Write("Are you checking out your shopping cart ? ( 'y' / 'n' ) : ");
                string confirmation = Console.ReadLine();

                switch (confirmation)
                {
                    // confirmed checkout
                    case "y":
                        
                        // check if user has enough balance
                        if (main_user.balance >= user_cart.calculate_cart())
                        {
                            Console.WriteLine("Cart succesfully checked out ! \n");

                            // clear cart
                            user_cart._clear();

                            // reduce user balance
                            main_user.balance -= user_cart.calculate_cart();
                        }
                        else 
                        {
                            Console.WriteLine("You don't have enough balance in your account ! \n");
                        }
                        break;
                    
                    // no to checkout
                    case "n":
                        break;
                    
                    // error catch all
                    default:
                        Console.WriteLine("command not recognized");
                        break;
                }
                break;


            // EXIT ------------------------
            case "exit": 
                Console.WriteLine("Exiting shop, have a nice 24 hours \n");
                break;

            
            // Unidentified command --------
            default:
                Console.WriteLine("Command cannot be identified \n");
                break;
                
        }
    }
}


// Create n connect Database ------------------------------------------------------------
public class Shop0DataContext : DbContext
{
    static readonly string connectionString = "Server=localhost; User ID=root; Password=godeater0; Database=Shop0";

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}


// Classes =======================================================================================    

// Creates product tables ---------------------------------------------------------------
public class Product
{
    public int ProductId { get; set; }
    public string name { get; set; } = "NoName";
    public int stock { get; set; }
    public double price { get; set; }
    public string desc { get; set; } = "NoDesc";


    // method for shallow cloning product object 
    public object Shallowcopy() 
    { 
        return this.MemberwiseClone(); 
    } 
    
} 


// user class -------------------------------------------------------------------------
public class User (string _name, double _balance)
{
    public string name { get; set; } = _name;
    public double balance { get; set; } = _balance;
    public List<string> history { get; set; } = new List<string>();

}


// Transaction / cart -----------------------------------------------------------------
public class Transaction 
{

    // cart list
    public List<Product> cart { get;} = new List<Product>();

    // cart price
    private double cart_price;


    // add a product to cart
    public void add_product(Product _product) {

        cart.Add(_product);
        //Console.Write("product object '" +_product+ "' has been added to cart class" );

    }

    // remove product from cart
    public void remove_product(string _product_name) 
    {
        // check if product exists in cart
        foreach (var item in cart)
        {
            
            // if existst remove from carty
            if (item.name == _product_name) 
            {

                cart.RemoveAll(x => x.name == item.name);

                Console.Write("\n The item '" + _product_name + "' has been removed from your cart \n");
                break;

            }
            else {

                // item inputed not found
                Console.Write("\n The item '" + _product_name + "' doesn't exists \n");
                break;
            }
        }
    }


    // print all products in cart
    public void print_cart()
    {

        Console.WriteLine(
            "{0,15}\t{1,10}\t{2,20}",
            "Name", "Price","Description"
            );

        foreach (var item in cart)
        {

            Console.WriteLine(
                "{0,15}\t{1,10}\t{2,20}",
                $"{item.name}", $"{item.price}", $"{item.desc}"
            );
        }

    }

    // checkout the cart ( Calculate all item price )
    public double calculate_cart()
    {   
        cart_price = 0;

        foreach (var item in cart)
        {
            cart_price += item.price;   
        }

        return cart_price;
    }

    // clear cart of everything
    public void _clear()
    {

        // clear items
        foreach (var item in cart)
        {
        cart.RemoveAll(x => x.name == item.name);
        }

        // clear cart price
        cart_price = 0;

    }
    
}

