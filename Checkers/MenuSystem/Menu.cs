namespace MenuSystem

{
    public class Menu
    {
        
        private List<MenuItem> MenuItems { get; } = new();
        

        private readonly MenuLevel _menuLevel;
        private readonly string _menuTitle;
        private readonly Action[] _actions;

        public Menu(MenuLevel level, string menuTitle)
        {
            _menuLevel = level;
            _menuTitle = menuTitle;
            _actions = GetActions();
        }


        private Action[] GetActions()
        {
            Action[] actionsLevel2Plus = { Action.Exit, Action.Back, Action.ReturnToMainMenu, Action.Select };
            Action[] actionsLevel1 = { Action.Exit, Action.Back, Action.Select };
            Action[] actionsLevel0 = { Action.Exit, Action.Select };
            
            switch (_menuLevel)
            {
                case MenuLevel.Level0:
                    return actionsLevel0;
                case MenuLevel.Level1:
                    return actionsLevel1;
                case MenuLevel.Level2Plus:
                    return actionsLevel2Plus;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddMenuItem(MenuItem item)
        {
            MenuItems.Add(item);

            /*
             * Check for correct label.
            */

            if (item.Label == "") // Label can't be empty.
            {
                throw new ArgumentException("Empty label");
            }


            if (item.Label.Length > 200) // Label can't be longer than 50 chars.
            {
                throw new ArgumentException($"Label too long. 200 chars max, you have {item.Label.Length}");
            }
        }
        
        public void ClearMenuItems()
        {
            MenuItems.Clear();
        }

        public Action RunMenu()
        {
            var userAction = Action.None;

            int currentlySelectedItemIndex = 0;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(_menuTitle);
                Console.ForegroundColor = ConsoleColor.Cyan;

                // loop over menuItems with index. Draw arrow if index is same as currentlySelectedItemIndex.
                foreach (var (index, item) in MenuItems.Select((item, index) => (index, item)))
                {
                    if (index == currentlySelectedItemIndex)
                    {
                        Console.WriteLine($"-> {item.Label}");
                    }
                    else
                    {
                        Console.WriteLine($"   {item.Label}");
                    }
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" ");
                Console.WriteLine("ARROW KEYS TO NAVIGATE, ENTER TO SELECT");
                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine("X) exit");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine("BACKSPACE) back");
                        Console.WriteLine("X) exit");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine("BACKSPACE) Return to previous");
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) exit");
                        break;
                    default:
                        throw new Exception("Unknown menu depth!");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("==========================================");
                Console.ResetColor();
                Console.WriteLine("");
                do
                {
                    var keyPress = Console.ReadKey(true);
                    // if arrow down, increment currentlySelectedItemIndex
                    if (keyPress.Key == ConsoleKey.DownArrow)
                    {
                        currentlySelectedItemIndex++;
                        if (currentlySelectedItemIndex >= MenuItems.Count)
                        {
                            currentlySelectedItemIndex = 0;
                        }

                        break;
                    }

                    // if arrow up, decrement currentlySelectedItemIndex
                    if (keyPress.Key == ConsoleKey.UpArrow)
                    {
                        currentlySelectedItemIndex--;
                        if (currentlySelectedItemIndex < 0)
                        {
                            currentlySelectedItemIndex = MenuItems.Count - 1;
                        }

                        break;
                    }
                    
                    if (keyPress.Key == ConsoleKey.Enter)
                    {
                        var menuItem = MenuItems[currentlySelectedItemIndex];
                        var userChoice = menuItem.MethodToExecute?.Invoke().ToString();
                        
                        if (Enum.TryParse(userChoice, out Action action))
                        {
                            userAction = action;
                            break;
                        }

                        throw new Exception("Unknown action!");
                    }
                    
                    if (keyPress.Key == ConsoleKey.X)
                    {
                        userAction = Action.Exit;
                    }
                    
                    if (keyPress.Key == ConsoleKey.Backspace)
                    {
                        userAction = Action.Back;
                    }
                    
                    if (keyPress.Key == ConsoleKey.M)
                    {
                        userAction = Action.ReturnToMainMenu;
                    }
                    
                    if (!_actions.Contains(userAction))
                    {
                        userAction = Action.None;
                    }
                    
                } while (!_actions.Contains(userAction));

                
                if (userAction == Action.Exit)
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        Console.WriteLine("\nClosing down......");
                    }

                    return Action.Exit;
                }
                
                if (userAction == Action.Back)
                {
                    return Action.None;
                }
                
                if (userAction == Action.ReturnToMainMenu)
                {

                    if (_menuLevel == MenuLevel.Level0)
                    {
                        continue;
                    }

                    return Action.ReturnToMainMenu;

                }
            } while (true);
        }
    }
}