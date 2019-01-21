using QuartzBaseMacroProgramWPF.Utils;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace QuartzBaseMacroProgramWPF
{
    public class TrayService : IDisposable
    {
        private string TooltipText
        {
            get
            {
                return "매크로";
            }
        }

        private static TrayService instance = new TrayService();
        public static NotifyIcon notifyIcon;
        private ContextMenu menu;
        private MenuItem submenu;
        private MenuItem m1;
        private MenuItem m2;
        private MenuItem m3;
        private MenuItem m4;

        private TrayService()
        {
            Debug.WriteLine("start tray");
            notifyIcon = new NotifyIcon();
            menu = new ContextMenu();
            submenu = new MenuItem()
            {
                Text = "작업 목록",
                Name = "SubMenu"
            };
            m1 = new MenuItem()
            {
                Text = "매크로 종료",
                Name = "ExitButton"
            };
            m2 = new MenuItem()
            {
                Text = "매크로 일시 중지",
                Name = "ExitButton"
            };
            m3 = new MenuItem()
            {
                Text = "설정 파일 열기",
                Name = "OpenSettingFile"
            };
            m4 = new MenuItem()
            {
                Text = "설정 갱신",
                Name = "ResetSetting"
            };
            notifyIcon.Icon = Properties.Resources.Icon3;
            notifyIcon.Visible = true;

            m1.Click += delegate
            {
                System.Windows.Application.Current.Shutdown();
            };

            m2.Click += delegate
            {
                GlobalVars.scheduler.PauseandResume();
            };

            m3.Click += delegate
            {
                Process.Start("notepad.exe", "settings/data.json");
            };

            m4.Click += delegate
            {
                GlobalVars.SetQuartzJobs();
            };

            menu.MenuItems.Add(submenu);
            menu.MenuItems.Add(m4);
            menu.MenuItems.Add(m3);
            menu.MenuItems.Add(m2);
            menu.MenuItems.Add(m1);

            notifyIcon.ContextMenu = menu;

            notifyIcon.Text = "키보드 매크로 스케줄러 - 실행중";
            menu.Popup += (s, e) => ReloadSubMenu();
        }

        public static TrayService GetInstance()
        {
            return instance;
        }

        public void ReloadSubMenu()
        {
            submenu.MenuItems.Clear();
            int i = 0;
            foreach (Quartz.Impl.Triggers.CronTriggerImpl x in GlobalVars.scheduler.GetTriggers())
            {
                MenuItem newItem = new MenuItem($"{i}: {x.Description}, {CronExpressionDescriptor.ExpressionDescriptor.GetDescription(x.CronExpressionString)}");
                /*,다음 실행 시간 {x.GetNextFireTimeUtc().Value.LocalDateTime.ToString()}*/
                newItem.Enabled = false;
                submenu.MenuItems.Add(newItem);
                i++;
            }

            if (!GlobalVars.scheduler.InStandbyMode)
            {
                m2.Text = "매크로 일시중지";
            }
            else
            {
                m2.Text = "매크로 재시작";
            }

            m2.Enabled = m4.Enabled = !GlobalVars.istimerticking;
        }

        public static void ShowMSG(string msg)
        {
            if (notifyIcon.Visible)
            {
                notifyIcon.BalloonTipText = msg;
                notifyIcon.BalloonTipTitle = System.AppDomain.CurrentDomain.FriendlyName;
                notifyIcon.ShowBalloonTip(1);
            }
        }

        public void HideSelf()
        {
            notifyIcon.Visible = false;
        }

        public void TurnOn()
        {
            notifyIcon.Visible = true;
        }

        #region IDisposable Support

        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m1.Dispose();
                    m2.Dispose();
                    m3.Dispose();
                    m4.Dispose();
                    submenu.MenuItems.Clear();
                    submenu.Dispose();
                    menu.Dispose();
                    instance = null;
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~TrayService() {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}