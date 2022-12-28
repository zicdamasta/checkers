using MenuSystem;
using Action = MenuSystem.Action;



var menu = new Menu(MenuLevel.Level0, "==============> MAIN MENU <==============");
menu.AddMenuItem(new MenuItem("New game human vs human.", MethodA));

menu.AddMenuItem(new MenuItem("Option 2", MethodB));

menu.AddMenuItem(new MenuItem("Option 3", SubmenuA));

Action MethodA()
{
    Console.WriteLine("Method A!!!!!");
    // press any key to continue
    Console.ReadKey();
    return Action.None;
}

Action MethodB()
{
    Console.WriteLine("Method B!!!!!");
    // press any key to continue
    Console.ReadKey();
    return Action.None;
}

Action SubmenuA()
{
    Console.Clear();
    var menu2 = new Menu(MenuLevel.Level1, "==============> SUBMENU A <==============");
    menu2.AddMenuItem(new MenuItem("Submenu A Option 1", MethodA));
    menu2.AddMenuItem(new MenuItem("Submenu A Option 2", MethodB));
    menu2.AddMenuItem(new MenuItem("Submenu B", SubmenuB));
    return menu2.RunMenu();
}

Action SubmenuB()
{
    Console.Clear();
    var menu3 = new Menu(MenuLevel.Level2Plus, "==============> SUBMENU B <==============");
    menu3.AddMenuItem(new MenuItem("Submenu A Option 1", MethodA));
    menu3.AddMenuItem(new MenuItem("Submenu A Option 2", MethodB));
    menu3.AddMenuItem(new MenuItem("Submenu A Option 2", SubmenuC));
    return menu3.RunMenu();
}

Action SubmenuC()
{
    Console.Clear();
    var menu4 = new Menu(MenuLevel.Level2Plus, "==============> SUBMENU C <==============");
    menu4.AddMenuItem(new MenuItem("Submenu A Option 1", MethodA));
    menu4.AddMenuItem(new MenuItem("Submenu A Option 2", MethodB));
    return menu4.RunMenu();
}



var menu2 = new Menu(MenuLevel.Level0, "==============> SECOND MENU <==============");
menu2.AddMenuItem(new MenuItem("Option 1", (() =>
{
    Console.WriteLine("Option 1");
    return Action.None;
})));

menu2.AddMenuItem(new MenuItem("Option 2", (() =>
{
    Console.WriteLine("Option 2");
    return Action.None;
})));

menu.RunMenu();