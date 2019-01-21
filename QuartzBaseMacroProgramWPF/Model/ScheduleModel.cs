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
        public List<SequenceKeyModel> sequencekey;
    }

    public class PressKeyScheduleModel
    {
        public string sendkey;
        public string cronexpression;
    }

    public class PressKeyMultiScheduleModel
    {
        public List<string> sendkeys;
        public string cronexpression;
    }

    public class SequenceKeyModel
    {
        public string holdkey;
        public List<int> unholdafter;
        public List<int> reholdafter;
        public List<string> sendkeys;
        public double interval;
        public string cronexpression;
    }

    public class SequenceKeyData
    {
        public int holdkey;
        public List<int> unholdafter;
        public List<int> reholdafter;
        public List<int> sendkeys;
        public TimeSpan interval;
        public bool iscombomode;
        public bool isholding;
    }

}
