using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace Logik.Pw.Logik.ViewModel
{

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ImportSyncVM>();
            SimpleIoc.Default.Register<RandomCenterVM>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ImportSyncVM ImpSnyc => ServiceLocator.Current.GetInstance<ImportSyncVM>();

        public RandomCenterVM RndCent => ServiceLocator.Current.GetInstance<RandomCenterVM>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}