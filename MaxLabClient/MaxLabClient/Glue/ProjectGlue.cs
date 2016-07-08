using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using TimeToShineClient.Model.Service;
using TimeToShineClient.View;
using TimeToShineClient.View.Home;
using XamlingCore.Portable.Contract.Downloaders;
using XamlingCore.UWP.Glue;

namespace TimeToShineClient.Glue
{
    public class ProjectGlue : UWPGlue
    {
        public override void Init()
        {
            base.Init();

            XUWPCoreAutoRegistration.RegisterAssembly(Builder, typeof(MainHomeViewModel));

            Builder.RegisterAssemblyTypes(typeof (MainHomeViewModel).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Repo"))
                .AsImplementedInterfaces()
                .SingleInstance();

            //Builder.RegisterAssemblyTypes(typeof(MainHomeViewModel).GetTypeInfo().Assembly)
            //    .Where(t => t.Name.EndsWith("ViewModel"))
            //    .AsSelf();


            Builder.RegisterType<TransferConfigService>().As<IHttpTransferConfigService>();

            Container = Builder.Build();
        }
    }
}
