using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using SmartBot.Database;

namespace HearthstoneMulligan.USER_GUI.CoachMode
{
    /// <summary>
    /// Interaction logic for CoachWindow.xaml
    /// </summary>
    public partial class CoachWindow
    {
        public CoachWindow()
        {
            ResourceDictionary Controls = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.Absolute)
            };
            ResourceDictionary Fonts = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.Absolute)
            };
            ResourceDictionary Colors = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.Absolute)
            };
            ResourceDictionary BlueAccent = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.Absolute)
            };
            ResourceDictionary BaseLightAccent = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.Absolute)
            };

            Application.Current.Resources.MergedDictionaries.Add(Fonts);
            Application.Current.Resources.MergedDictionaries.Add(Colors);
            Application.Current.Resources.MergedDictionaries.Add(BlueAccent);
            Application.Current.Resources.MergedDictionaries.Add(BaseLightAccent);
            Application.Current.Resources.MergedDictionaries.Add(Controls);

            InitializeComponent();

            DataContext = new MainViewModel();

            minProb_procBar.Value = ValueReader.MinProbabilityToReplace;
            minProb_lbl.Content = ValueReader.MinProbabilityToReplace + " %";

            DeckCalculation deckCalculation = new DeckCalculation(ValueReader.MinProbabilityToReplace);
            Tuple<int, float> treeResult = deckCalculation.GenerateTreeWhiteListDraw();
            realProb_procBar.Value = treeResult.Item2;
            realProb_lbl.Content = treeResult.Item2;

            int normalMinionCount = MainLists.HandCards_BoardCards.Count(x => new NeutralMinion(x).BoardCard != null &
                NeutralMinion.WouldTakeMinion(x) && !MainLists.whiteList.Contains(x.Id.ToString()));

            //less medium cards than actually having possible
            if (treeResult.Item1 /*prospective refused*/ < normalMinionCount)
            {
                int deltaReplaceCount = normalMinionCount - treeResult.Item1;

                int i = 1;
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (CardTemplate badCard in MainLists.HandCards_BoardCards.
                    Where(x => new NeutralMinion(x).BoardCard != null && NeutralMinion.WouldTakeMinion(x) &&
                        !MainLists.whiteList.Contains(x.Id.ToString())).
                    OrderBy(x => new NeutralMinion(x).CardValue).TakeWhile(x => i <= deltaReplaceCount))
                {
                    string cardString = badCard.Name + " (" + badCard.Id + ")";
                    textBox1.AppendText(cardString + "\n");
                    ++i;
                }

                replaceCount_info.Content = "...by being able to replace " + deltaReplaceCount + " cards in hand";
            }

        }
    }
    public class MainViewModel
    {
        // ReSharper disable once MemberCanBePrivate.Global => PROPERTY BINDING
        // ReSharper disable once CollectionNeverQueried.Global => PROPERTY BINDING
        public ObservableCollection<ManaCurve> ManaCurves { get; } = new ObservableCollection<ManaCurve>();

        public MainViewModel()
        {
            //cost in names//index equal to cost
            int[] manaCurves = DeckCalculation.GetCurves();

            for (int i = 0; i < 11; i++)
            {
                int currentCost = i;
                ManaCurve curvei = new ManaCurve { Amount = manaCurves[i], Name = currentCost.ToString() };
                ManaCurves.Add(curvei);
            }
        }
    }

    public class ManaCurve : INotifyPropertyChanged
    {
        private string _name;
        private int _amount;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public int Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                _amount = value;
                NotifyPropertyChanged("Amount");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
