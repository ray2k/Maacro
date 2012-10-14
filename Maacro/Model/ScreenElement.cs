using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


namespace Maacro.Model
{
    public class ScreenElement : ReactiveObject
    {
        private string _Name;
        private int _X;
        private int _Y;
        private ScreenElementType _ElementType;

        public ScreenElement(string name)
            : this()
        {
            this.Name = name;            
        }

        public ScreenElement()
        {
            _X = 0;
            _Y = 0;
        }

        public bool HasLocationSet
        {
            get { return _X != 0 && _Y !=0; }
        }

        internal static ScreenElement Create(ScreenElementType type, string name)
        {
            return new ScreenElement() { ElementType = type, Name = name };
        }

        public ScreenElementType ElementType
        {
            get { return _ElementType; }
            set { _ElementType = this.RaiseAndSetIfChanged(se => se.ElementType, value); }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = this.RaiseAndSetIfChanged(se => se.Name, value); }
        }

        public int X
        {
            get { return _X; }
            set { _X = this.RaiseAndSetIfChanged(se => se.X, value); }
        }

        public int Y
        {
            get { return _Y; }
            set { _Y = this.RaiseAndSetIfChanged(se => se.Y, value); }
        }      

        public static IEnumerable<ScreenElement> GetDefaults()
        {
            return new List<ScreenElement>()
            {
                ScreenElement.Create(ScreenElementType.JetBay1, "Jet Bay 1"),
                ScreenElement.Create(ScreenElementType.JetBay2, "Jet Bay 2"),
                ScreenElement.Create(ScreenElementType.JetBay3, "Jet Bay 3"),
                ScreenElement.Create(ScreenElementType.JetBay4, "Jet Bay 4"),
                ScreenElement.Create(ScreenElementType.JetBay5, "Jet Bay 5"),
                ScreenElement.Create(ScreenElementType.JetBay6, "Jet Bay 6"),
                ScreenElement.Create(ScreenElementType.JetBay7, "Jet Bay 7"),
                ScreenElement.Create(ScreenElementType.JetBay8, "Jet Bay 8"),

                ScreenElement.Create(ScreenElementType.HeroSlot1, "Hero Slot 1"),
                ScreenElement.Create(ScreenElementType.HeroSlot2, "Hero Slot 2"),
                ScreenElement.Create(ScreenElementType.HeroSlot3, "Hero Slot 3"),
                ScreenElement.Create(ScreenElementType.HeroSlot4, "Hero Slot 4"),
                ScreenElement.Create(ScreenElementType.HeroSlot5, "Hero Slot 5"),

                ScreenElement.Create(ScreenElementType.PrevHeroPage, "Prev Hero Page"),
                ScreenElement.Create(ScreenElementType.NextHeroPage, "Next Hero Page"),
                ScreenElement.Create(ScreenElementType.ConfirmButton, "Confirm Button"),
                ScreenElement.Create(ScreenElementType.CollectAll, "Collect All")
            };
        }

        public static ScreenElementType GetScreenElementTypeForSlotNumber(int slotNumber)
        {
            switch (slotNumber)
            {
                case 1:
                    return ScreenElementType.HeroSlot1;
                case 2:
                    return ScreenElementType.HeroSlot2;
                case 3:
                    return ScreenElementType.HeroSlot3;
                case 4:
                    return ScreenElementType.HeroSlot4;
                case 5:
                    return ScreenElementType.HeroSlot5;
                default:
                    throw new InvalidOperationException("Slot number must be an integer from 1 through 5");
            }
        }

        public static ScreenElementType GetScreenElementTypeForBayNumber(int bayNumber)
        {
            switch (bayNumber)
            {
                case 1:
                    return ScreenElementType.JetBay1;
                case 2:
                    return ScreenElementType.JetBay2;
                case 3:
                    return ScreenElementType.JetBay3;
                case 4:
                    return ScreenElementType.JetBay4;
                case 5:
                    return ScreenElementType.JetBay5;
                case 6:
                    return ScreenElementType.JetBay6;
                case 7:
                    return ScreenElementType.JetBay7;
                case 8:
                    return ScreenElementType.JetBay8;
                default:
                    throw new InvalidOperationException("Bay number must be an integer from 1 through 8");
            }
        }

        
    }
}
