using GalaSoft.MvvmLight;
using Logik.Pw.Logik.Items;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Logik.Wo.Logik.ViewModel
{

        public class MainViewModel : ViewModelBase
        {

            public ObservableCollection<PwEintrag> MainListe { get; set; }
            public PwEintrag AktEintrag
            {
                get
                {
                    var _tmpErg = UiViewListe?.CurrentItem as PwEintrag;
                    if (_tmpErg != null)
                    {
                        // logik zum einzel anzeigen
                    }
                    return _tmpErg;
                }
                set
                {
                    UiViewListe.MoveCurrentTo(value);
                    RaisePropertyChanged();
                }
            }

            //  private ListCollectionView _UiViewListe;
            public ListCollectionView UiViewListe { get; set; }



            public MainViewModel()
            {
                if (IsInDesignMode)
                {
                    // Code runs in Blend --> create design time data.
                    string Fehlermeldung = "";
                }
                MainListe = new ObservableCollection<PwEintrag>();
                initzialise();
            }


            public void initzialise()
            {

                #region ListCollectionView Initailisierung

                UiViewListe = CollectionViewSource.GetDefaultView(MainListe) as ListCollectionView;
                foreach (var item in MainListe) // die vom System rein geladenen Daten müssen das OnPropertyChangeEvent "registrieren"
                {
                    item.PropertyChanged += PersonInfosPropertyAnders;
                }
                UiViewListe.CurrentChanged += (s, e) =>
                {
                    RaisePropertyChanged(() => AktEintrag);
                };
                MainListe.CollectionChanged += (s, e) =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (INotifyPropertyChanged added in e.NewItems)
                        {
                            added.PropertyChanged += PersonInfosPropertyAnders;
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (INotifyPropertyChanged wiederweg in e.OldItems)
                        {
                            wiederweg.PropertyChanged -= PersonInfosPropertyAnders;
                        }
                    }
                };
                #endregion

                AktEintrag = UiViewListe?.CurrentItem as PwEintrag;
            }

            private void PersonInfosPropertyAnders(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(AktEintrag.HasErrors)) // würde die ganze zeit sonst beim initialisieren anschlagen und den RAM voll laufen
                {
                    return;
                }
                if (UiViewListe.IsEditingItem || UiViewListe.IsAddingNew) // nicht wechseln solange jemand darin rum schreibt
                {
                    return;
                }
                UiViewListe.Refresh();
            }

        }
    }