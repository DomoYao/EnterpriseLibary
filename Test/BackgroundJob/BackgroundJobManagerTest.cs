using Enterprises.Framework.BackgroundJobs;
using Enterprises.Framework.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.BackgroundJob
{
    public class BackgroundJobsTest
    {
        public void BackgroundJobManagerTest()
        {
            IIocManager iocResolver = new IocManager();
            // 注册服务
            iocResolver.Register<SimpleSendEmailJob>(DependencyLifeStyle.Transient);
            IBackgroundJobStore jobStore = new InMemoryBackgroundJobStore();
            IBackgroundJobManager _backgroundJobManager = new BackgroundJobManager(iocResolver, jobStore, new Enterprises.Framework.Threading.AbpTimer());
            _backgroundJobManager.Enqueue<SimpleSendEmailJob, SimpleSendEmailJobArgs>(
            new SimpleSendEmailJobArgs
            {
                Subject = "邮件主题",
                Body = "测试邮件",
                SenderUserId = 1000,
                TargetUserId = 2000
            });


            _backgroundJobManager.Enqueue<SimpleSendEmailJob, SimpleSendEmailJobArgs>(
           new SimpleSendEmailJobArgs
           {
               Subject = "邮件主题2",
               Body = "测试邮件2",
               SenderUserId = 1000,
               TargetUserId = 2000
           },BackgroundJobPriority.Normal,TimeSpan.FromSeconds(10));

            _backgroundJobManager.Start();

            Console.WriteLine($"datetime={DateTime.Now} end send Email ");
        }
    }

    public class SimpleSendEmailJob : BackgroundJob<SimpleSendEmailJobArgs>, ITransientDependency
    {

        public SimpleSendEmailJob()
        {

        }

        public override void Execute(SimpleSendEmailJobArgs args)
        {
            Console.WriteLine($"datetime={DateTime.Now} Begin send Email ");
            Task.Delay(1000);
            Console.WriteLine($"datetime={DateTime.Now} Send Email form {args.SenderUserId} to {args.TargetUserId} Subject is{args.Subject} body is {args.Body}");
        }
    }

    [Serializable]
    public class SimpleSendEmailJobArgs
    {
        public long SenderUserId { get; set; }

        public long TargetUserId { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
