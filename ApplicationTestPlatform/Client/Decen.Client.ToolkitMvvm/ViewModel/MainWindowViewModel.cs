using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Decen.Common.Extensions;
using Decen.Common.Logs.Logging.LogImplement;
using Decen.Core.Laboratory.Entity.RequestDtos;
using Decen.Core.Laboratory.Entity.ResponseDtos;
using Decen.Core.Laboratory.Service.IService;
using Decen.Core.Laboratory.Service.RepositoryImpl;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decen.Client.ToolkitMvvm.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {

        private readonly ICompanyService _companyService;
        public MainWindowViewModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }


        #region 联动修改文本内容
        [ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(Caption))]
        private string title = "你好!";

        partial void OnTitleChanged(string value)
        {
            OnPropertyChanged(nameof(Caption));
        }

        public string Caption => $"Title:{Title}";

        [RelayCommand]
        private void ButtonClick()
        {
            //Title = "Goodbye";

            //Demo demo = new Demo();
            //demo.ShowDialog();

            //ListView listView = new ListView();
            //listView.ShowDialog();

            //CompanyRepository companyRepository = new CompanyRepository();
            CompanyRequestDto requestDto = new CompanyRequestDto { PageIndex = 1, PageSize = 10 };

            //////单个查询
            var list = _companyService.GetCompanyAsync(requestDto);
            CompanyResponeDto result = list.Adapt<CompanyResponeDto>();


            //日志
            SessionLogs.Initialize("console_log.txt");


        }
        #endregion


        #region PropertyChanged.Fody
        [AlsoNotifyFor(nameof(Age))]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string FullName => $"{FirstName}{LastName}";

        [DependsOn(nameof(FirstName), nameof(LastName))]
        public int Age { get; set; }

        void OnFirstNameChanged()
        {
            Age = 18;
        }
        #endregion

    }
}
