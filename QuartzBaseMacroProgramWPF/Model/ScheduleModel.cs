using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzBaseMacroProgramWPF.Model
{
    public class ScheduleModel
    {
        public List<PressKeyScheduleModel> presskey;
        public List<PressKeyMultiScheduleModel> presskeymulti;
    }

    public class PressKeyScheduleModel
    {
        public int sendkey;
        public string cronexpression;
    }

    public class PressKeyMultiScheduleModel
    {
        public List<int> sendkeys;
        public string cronexpression;
    }

    public class ExecuteProcessScheduleModel
    {
        public string fullpath;
        public string cronexpression;
    }

}
