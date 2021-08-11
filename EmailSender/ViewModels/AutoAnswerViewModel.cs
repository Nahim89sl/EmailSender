using StyletIoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSender.ViewModels
{
    public class AutoAnswerViewModel
    {
        #region

        private IContainer _ioc;

        #endregion

        #region Constructor

        public AutoAnswerViewModel(IContainer ioc)
        {
            _ioc = ioc;
        }

        #endregion
    }
}
