using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KiwoomCode;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

namespace StockTrader
{
    public enum 주문구분
    {
        신규매수 = 1,
        신규매도 = 2,
        매수취소 = 3,
        매도취소 = 4,
    }

    public enum 주문단계
    {
        주문요청 = 1,
        주문접수 = 2,
        모든체결완료 = 3
    }

    public enum 실시간종류
    {
        조건검색 = 1,
        주식시세 = 2,
    }

    public enum 매수조건
    {
        현재가 = 0,
        P1아래, // 1% 아래
        P2아래, // 2% 아래
        P3아래, // 3% 아래
    }


    public partial class FormMain : Form
    {

        #region [변수선언]
        private const int CONST_편입매수시기강도_삭제 = -100; //-15;
        private const int CONST_편입매수시기강도_매수 = 100; //5;

        private List<ConditionList> _ConditionList = new List<ConditionList>();
        private List<등록된조건옵션> _ConRegistList = new List<등록된조건옵션>();

        // 등록된조건옵션 1 : 조건검색종목 n (조건번호로 조인)
                
        private List<조건검색종목> _ConSearchItemList = new List<조건검색종목>();

        private List<매도종목> _list매도종목 = new List<매도종목>();

        private List<실시간등록> _list실시간등록 = new List<실시간등록>();

        private Dictionary<string, List<실시간데이터>> _dic실시간데이터 = new Dictionary<string, List<실시간데이터>>();

        private const string _주문화면 = "4989";
        private const string _호가화면 = "4990";
        private const string _잔고화면 = "4991";
        private const string _기본정보화면 = "1111";
        private const string _조건검색화면 = "4992";
        private const string _실시간화면 = "0341";
        
        private const string _장시작시간 = "09:00:00";
        private const string _장마감시간 = "15:30:00";

        private string _ServerGubun;
        private string _str계좌번호;
        
        public UInt32 _ProcDelay;
        public UInt32 _GridViewDelay;

        private bool _b실시간등록타입 = false;
        private bool _b결과보고;
        private bool _bBackLoop = true;
        #endregion [변수선언]


        #region [매수_매도 아이템 리스트]
        private const string _col아이템_종목번호 = "종목번호";
        private const string _col아이템_종목명 = "종목명";
        private const string _col아이템_매입가 = "매입가";
        private const string _col아이템_현재가 = "현재가";        
        private const string _col아이템_보유수량 = "보유수량";
        private const string _col아이템_수익률 = "수익률";
        private const string _col아이템_매수단계 = "매수단계";
        private const string _col아이템_매도단계 = "매도단계";
        private const string _col아이템_시간 = "시간";
        private const string _col아이템_편입매수시기강도 = "편입매수시기강도";
        private const string _col아이템_고가 = "고가";
        private const string _col아이템_저가 = "저가";
        private const string _col아이템_매수체결시간 = "매수체결시간";
        private DataTable _dt매수매도아이템리스트 = new DataTable();
        #endregion [매수_매도 아이템 리스트]


        #region [조건검색 종목리스트]
        private const string _col조건검색_시간 = "시간";
        private const string _col조건검색_종목번호 = "종목번호";
        private const string _col조건검색_종목명 = "종목명";
        private const string _col조건검색_조건명 = "조건명";
        private DataTable _dt조건검색종목리스트 = new DataTable();
        #endregion [조건검색 종목리스트]



        public FormMain()
        {
            InitializeComponent();

            DoubleBuffered = true;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, grd_아이템리스트, new object[] { true });
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, grd_종목리스트, new object[] { true });


            #region [매수_매도 아이템 리스트]
            _dt매수매도아이템리스트.Columns.Add(_col아이템_종목번호, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_종목명, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_매입가, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_현재가, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_보유수량, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_수익률, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_매수단계, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_매도단계, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_시간, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_편입매수시기강도, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_고가, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_저가, typeof(string));
            _dt매수매도아이템리스트.Columns.Add(_col아이템_매수체결시간, typeof(string));
            grd_아이템리스트.DataSource = _dt매수매도아이템리스트;
            grd_아이템리스트.AutoResizeColumns();
            grd_아이템리스트.AutoResizeRows();
            #endregion [매수_매도 아이템 리스트]


            #region [조건검색 종목리스트]
            _dt조건검색종목리스트.Columns.Add(_col조건검색_시간, typeof(string));
            _dt조건검색종목리스트.Columns.Add(_col조건검색_종목번호, typeof(string));
            _dt조건검색종목리스트.Columns.Add(_col조건검색_종목명, typeof(string));
            _dt조건검색종목리스트.Columns.Add(_col조건검색_조건명, typeof(string));
            grd_종목리스트.DataSource = _dt조건검색종목리스트;
            //grd_종목리스트.AutoResizeColumns();
            grd_종목리스트.AutoResizeRows();
            #endregion [조건검색 종목리스트]
            
        }


        #region [폼 이벤트]
        private void FormMain_Load(object sender, EventArgs e)
        {   
            cbo_매수조건.SelectedIndex = 0;
            cbo_매수금액.SelectedIndex = 4;

            cbo_잔고수익률.SelectedIndex = 2;
            cbo_잔고손절률.SelectedIndex = 5;
            cbo_조건식수익률.SelectedIndex = 2;
            cbo_조건식손절률.SelectedIndex = 0;

            Application.Idle += Application_Idle;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            //Idle이벤트를 없앤다.
            Application.Idle -= Application_Idle;

            로그인();

            SetMicroTime();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _bBackLoop = false;
            EndMicroTime();
            backgroundWorker.CancelAsync();

            로그아웃();
        }

        private void cbo_계좌정보_SelectedIndexChanged(object sender, EventArgs e)
        {
            _str계좌번호 = cbo_계좌정보.SelectedItem.ToString();
        }


        #region [추가된 아이템 리스트]
        private void btn_리스트조회_Click(object sender, EventArgs e)
        {
            f아이템리스트to테이블();
        }

        private void btn_선택매도_Click(object sender, EventArgs e)
        {
            // 장시간 체크
            // 10:00 ~ 15:00
            if (f장시간체크() == false) return;


            DataGridViewRow dr = grd_아이템리스트.SelectedRows[0];
            string str종목코드 = dr.Cells[_col아이템_종목번호].Value.ToString();

            int findIndex = f조건검색종목찾기(str종목코드);
            if (findIndex != -1)
            {
                조건검색종목 item = _ConSearchItemList[findIndex];
                if (item.n매도주문단계 > 0) return;
                if (item.n현재가 == 0) return;

                f지정가매도(str종목코드, item.n보유수량, item.n현재가, ref item.n매도주문단계);
            }
        }

        private void f아이템리스트to테이블()
        {
            this.Invoke(new Action(delegate ()
            {
                try
                {
                    bool bAutoResize = false;
                    List<조건검색종목> listItem = _ConSearchItemList.ToList();

                    foreach (조건검색종목 item in listItem)
                    {
                        int rowIndex = f테이블아이템찾기(_dt매수매도아이템리스트, item.str종목코드);
                        if (rowIndex == -1)
                        {
                            bAutoResize = true;
                            _dt매수매도아이템리스트.Rows.Add(item.str종목코드,
                                            item.str종목명,
                                            item.n매입가,
                                            item.n현재가,
                                            item.n보유수량,
                                            string.Format("{0:F2}", item.d손익율),
                                            item.n매수주문단계,
                                            item.n매도주문단계,
                                            item.str시간,
                                            item.n편입매수시기강도,
                                            item.n고가,
                                            item.n저가,
                                            item.dt매수체결시간);
                        }
                        else
                        {
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_종목번호] = item.str종목코드;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_종목명] = item.str종목명;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_매입가] = item.n매입가;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_현재가] = item.n현재가;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_보유수량] = item.n보유수량;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_수익률] = string.Format("{0:F2}", item.d손익율);
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_매수단계] = item.n매수주문단계;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_매도단계] = item.n매도주문단계;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_시간] = item.str시간;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_편입매수시기강도] = item.n편입매수시기강도;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_고가] = item.n고가;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_저가] = item.n저가;
                            _dt매수매도아이템리스트.Rows[rowIndex][_col아이템_매수체결시간] = item.dt매수체결시간;


                            if (item.d손익율 > 0)
                            {
                                grd_아이템리스트[grd_아이템리스트.Columns[_col아이템_수익률].Index, rowIndex].Style.BackColor = Color.MistyRose;
                            }
                            else if (item.d손익율 < 0)
                            {
                                grd_아이템리스트[grd_아이템리스트.Columns[_col아이템_수익률].Index, rowIndex].Style.BackColor = Color.SkyBlue;
                            }

                        }
                    }

                    if (bAutoResize)
                    {
                        grd_아이템리스트.AutoResizeColumns();
                        grd_아이템리스트.AutoResizeRows();
                    }
                }
                catch (Exception) { }
                
            }));
        }
        #endregion [추가된 아이템 리스트]


        #region [계좌정보 관련 콘트롤]
        private void btn_잔고조회_Click(object sender, EventArgs e)
        {
            grd_잔고.Rows.Clear();
            string strAccountNum = cbo_계좌정보.SelectedItem.ToString();

            // 예수금상세현황요청
            axKHOpenAPI.SetInputValue("계좌번호", strAccountNum);
            int nRet = axKHOpenAPI.CommRqData("예수금상세현황요청", "OPW00001", 0, _잔고화면);
            if (!Error.IsError(nRet))
            {
                Logger(Log.로그창, "[예수금상세현황요청] " + Error.GetErrorMessage());
            }

            // 계좌평가잔고내역요청 - OPW00018 은 한번에 20개의 종목정보를 반환
            GetMyAccountState(0);
        }

        private void btn_자동주문_Click(object sender, EventArgs e)
        {
            string str수익률 = cbo_잔고수익률.SelectedItem.ToString();
            double d수익률 = Convert.ToDouble(str수익률);
            string str손절률 = cbo_잔고손절률.SelectedItem.ToString();
            double d손절률 = Convert.ToDouble(str손절률);

            foreach(DataGridViewRow row in grd_잔고.Rows)
            {
                bool bFlag = false;
                string str종목코드 = row.Cells["잔고_종목코드"].Value.ToString();

                int findIndex = f조건검색종목찾기(str종목코드);
                if (findIndex == -1)
                {
                    bFlag = true;
                }
                else
                {
                    bool b잔고매도 = _ConSearchItemList[findIndex].b잔고매도;
                    if(b잔고매도 == false)
                    {
                        bFlag = true;
                    }
                }

                if (bFlag)
                {
                    string str종목명 = row.Cells["잔고_종목명"].Value.ToString();
                    string str매입가 = row.Cells["잔고_매입가"].Value.ToString();
                    string str현재가 = row.Cells["잔고_현재가"].Value.ToString();
                    string str보유수량 = row.Cells["잔고_보유수량"].Value.ToString();

                    조건검색종목 ConItem = new 조건검색종목();
                    ConItem.str종목코드 = str종목코드;
                    ConItem.str종목명 = str종목명;
                    ConItem.n매입가 = Int32.Parse(str매입가.Replace(",", ""));
                    ConItem.n현재가 = Int32.Parse(str현재가.Replace(",", ""));
                    ConItem.n보유수량 = Int32.Parse(str보유수량);
                    ConItem.b잔고매도 = true;
                    ConItem.d잔고수익률 = d수익률;
                    ConItem.d잔고손절률 = d손절률;
                    ConItem.n매수주문단계 = -1;

                    _ConSearchItemList.Add(ConItem);

                    if (DateTime.Now.TimeOfDay < TimeSpan.Parse(_장시작시간))
                    {
                        _list실시간등록.Add(new 실시간등록(str종목코드, "", 실시간종류.주식시세));
                    }
                    else
                    {
                        f실시간시세등록(str종목코드);
                    }
                }
            }
        }
        #endregion [계좌정보 관련 콘트롤]


        #region [조건식 관련 콘트롤]
        private void btn_조건식등록_Click(object sender, EventArgs e)
        {
            int nCboIndex = cbo_조건식리스트.SelectedIndex;

            //중복검사
            for(int i=0; i<_ConRegistList.Count; i++)
            {
                int nSelConIndex = _ConditionList[nCboIndex].nIndex;
                if(nSelConIndex == _ConRegistList[i].n조건번호)
                {
                    MessageBox.Show("조건식 중복 등록입니다.");
                    return;
                }
            }

            int n조건번호 = _ConditionList[nCboIndex].nIndex;
            string str조건명 = _ConditionList[nCboIndex].strConditionName;

            매수조건 e매수조건 = (매수조건)cbo_매수조건.SelectedIndex;
            string str매수금액 = cbo_매수금액.SelectedItem.ToString();
            string str수익률 = cbo_조건식수익률.SelectedItem.ToString();
            string str손절률 = cbo_조건식손절률.SelectedItem.ToString();

            등록된조건옵션 ConRegist = new 등록된조건옵션();
            ConRegist.n조건번호 = n조건번호;
            ConRegist.str조건명 = str조건명;
            ConRegist.e매수조건 = e매수조건;
            ConRegist.n매수금액 = Convert.ToInt32(str매수금액.Replace(",", ""));
            ConRegist.d수익률 = Convert.ToDouble(str수익률);
            ConRegist.d손절률 = Convert.ToDouble(str손절률);
            ConRegist.b실시간 = true;

            _ConRegistList.Add(ConRegist);

            grd_조건.Rows.Add(n조건번호, str조건명,
                              string.Format("{0}:{1} [{2}]", e매수조건, str매수금액, str수익률),
                              "중지");

            grd_조건.AutoResizeColumns();
            grd_조건.AutoResizeRows();

            if (DateTime.Now.TimeOfDay < TimeSpan.Parse(_장시작시간))
            {
                _list실시간등록.Add(new 실시간등록(_ConditionList[nCboIndex].strConditionName, _ConditionList[nCboIndex].nIndex.ToString(), 실시간종류.조건검색));
            }
            else
            {
                f실시간조건검색등록(_ConditionList[nCboIndex].strConditionName, _ConditionList[nCboIndex].nIndex);
            }
        }

        private void grd_조건_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                // Button Clicked - Execute Code Here

                int nIndex = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells["Col_I"].Value.ToString());    // 조건명 인덱스
                string strConditionName = senderGrid.Rows[e.RowIndex].Cells["Col_조건명"].Value.ToString();    // 조건식 이름                

                string strText = senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (strText.Equals("중지"))
                {
                    RealConditionStop(strConditionName, nIndex);

                    senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "시작";
                }
                else if (strText.Equals("시작"))
                {
                    f실시간조건검색등록(strConditionName, nIndex);

                    senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "중지";
                }
            }
        }
        #endregion [조건식 관련 콘트롤]


        #endregion [폼 이벤트]



        #region 1ms Timer Tick
        //////////////////////////////////////////////////////////////////////////
        // P/Invoke declarations
        private int mTimerId;
        private TimerEventHandler mHandler;
        private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);
        private const int TIME_PERIODIC = 1;
        private const int EVENT_TYPE = TIME_PERIODIC;
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventHandler handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);
        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);
        //////////////////////////////////////////////////////////////////////////

        private void SetMicroTime()
        {
            timeBeginPeriod(1);
            mHandler = new TimerEventHandler(TimerCallback);
            mTimerId = timeSetEvent(1, 0, mHandler, IntPtr.Zero, EVENT_TYPE);
        }

        private void EndMicroTime()
        {
            mTimerId = 0;
            int err = timeKillEvent(mTimerId);
            timeEndPeriod(1); // 1ms
            // Ensure callbacks are drained
            Thread.Sleep(100);
        }

        //private int MonDelay;
        //public int OutSensor_OffCheckDelay;
        private void TimerCallback(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            if (_ProcDelay > 0) _ProcDelay--;
            if (_GridViewDelay > 0) _GridViewDelay--;
        }
        #endregion




        #region [공용 함수]
        /// <summary>
        /// 실시간 연결 종료
        /// </summary>
        private void DisconnectAllRealData()
        {
            //axKHOpenAPI.DisconnectRealData(_호가화면);
            //axKHOpenAPI.DisconnectRealData(_잔고화면);
            //axKHOpenAPI.DisconnectRealData(_주문화면);
            axKHOpenAPI.DisconnectRealData(_실시간화면);
            axKHOpenAPI.DisconnectRealData(_조건검색화면);
        }

        /// <summary>
        /// 로그 출력
        /// </summary>
        /// <param name="type"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Logger(Log type, string format, params object[] args)
        {
            this.Invoke(new Action(delegate ()
            {
                string dir체결내역 = Application.StartupPath + @"\log\체결내역\";
                string dir로그 = Application.StartupPath + @"\log\로그\";

                DirectoryInfo df = new DirectoryInfo(dir체결내역);
                if (!df.Exists)
                {
                    df.Create();
                }
                df = new DirectoryInfo(dir로그);
                if (!df.Exists)
                {
                    df.Create();
                }

                string message = string.Format(format, args);

                DateTime dt = DateTime.Now;
                message = dt.ToString("[yyyy-MM-dd HH:mm:ss] ") + message;

                string fileName = dt.ToString("yyyy-MM-dd") + ".log";

                switch (type)
                {
                    case Log.체결내역:
                        log_체결내역.Items.Add(message);
                        log_체결내역.SelectedIndex = log_체결내역.Items.Count - 1;

                        File.AppendAllText(dir체결내역 + fileName, message + "\r\n");
                        break;
                    case Log.로그창:
                        log_로그창.Items.Add(message);
                        log_로그창.SelectedIndex = log_로그창.Items.Count - 1;

                        File.AppendAllText(dir로그 + fileName, message + "\r\n");
                        break;
                }
            }));
        }

        private string changeFormat(string data, int percent = 0)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            string formatData = string.Empty;

            if (percent == 0)
            {
                int d = Convert.ToInt32(data);
                formatData = string.Format("{0:N0}", Math.Abs(d));
            }
            else if (percent == 1)
            {
                double f = Convert.ToDouble(data) / 100.0;
                formatData = string.Format("{0:F2}", f);
            }
            else if (percent == 2)
            {
                double f = Convert.ToDouble(data);
                formatData = string.Format("{0:F2}", f);
            }

            return formatData;
        }

        private string f종목코드변환(string str코드)
        {
            string str종목코드 = string.Empty;

            if (str코드.Length > 6)
            {
                str종목코드 = str코드.Substring(str코드.Length - 6);
            }
            else
            {
                str종목코드 = str코드;
            }

            return str종목코드;
        }

        private int f가격변환(string data)
        {
            if (string.IsNullOrEmpty(data)) return 0;

            int d = Convert.ToInt32(data);
            d = Math.Abs(d);
            return d;
        }

        private void RealConditionStop(string str조건명, int n조건번호)
        {
            axKHOpenAPI.SendConditionStop(_조건검색화면, str조건명, n조건번호);
        }

        private int f등록된조건옵션찾기(int n조건번호)
        {
            return _ConRegistList.FindIndex(x => x.n조건번호 == n조건번호);
        }

        private int f조건검색종목찾기(string str종목코드)
        {
            return _ConSearchItemList.FindIndex(x => x.str종목코드 == str종목코드);
        }

        private int f테이블아이템찾기(DataTable dt, string str종목코드)
        {
            DataRow dr = dt.AsEnumerable().FirstOrDefault(row => row["종목번호"].Equals(str종목코드));
            int index = dt.Rows.IndexOf(dr);
            return index;
        }

        private bool f장시간체크()
        {
            if (DateTime.Now.TimeOfDay > TimeSpan.Parse(_장마감시간)
                && _b결과보고 == true)
            {
                _b결과보고 = false;

                // 실시간 데이터 로그 저장
                // 종목코드 별로 저장
                string dir실시간데이터 = Application.StartupPath + @"\log\실시간데이터\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\";
                DirectoryInfo df = new DirectoryInfo(dir실시간데이터);
                if (!df.Exists)
                {
                    df.Create();
                }


                Dictionary<string, List<실시간데이터>> dicData = new Dictionary<string, List<실시간데이터>>(_dic실시간데이터);
                foreach (var dic in dicData)
                {
                    string str종목코드 = dic.Key;
                    string str종목명 = dic.Value[0].str종목명;
                    string fileName = str종목코드 + "_" + str종목명 + ".csv";


                    //public string s전일대비;        // 11
                    //public string s등락률;         // 12
                    //public string s매도호가;        // 27
                    //public string s매수호가;        // 28
                    //public string s누적거래량;       // 13
                    //public string s누적거래대금;      // 14
                    //public string s전일대비기호;      // 25
                    //public string s전일거래량대비_계약;  // 26
                    //public string s거래대금증감;          // 29
                    //public string s전일거래량대비_비율;  // 30
                    //public string s거래회전율;           // 31
                    //public string s거래비용;            // 32
                    //public string s상한가발생시간;     // 567
                    //public string s하한가발생시간;     // 568
                    //public string s체결량;             // 15
                    //public string s체결강도;            // 228

                    File.AppendAllText(dir실시간데이터 + fileName, 
                                    "시간,현재가,고가,저가," +
                                    "전일대비," +
                                    "등락률," +
                                    "매도호가," +
                                    "매수호가," +
                                    "누적거래량," +
                                    "누적거래대금," +
                                    "전일대비기호," +
                                    "전일거래량대비_계약," +
                                    "거래대금증감," +
                                    "전일거래량대비_비율," +
                                    "거래회전율," +
                                    "거래비용," +
                                    "상한가발생시간," +
                                    "하한가발생시간," +
                                    "체결량," +
                                    "체결강도," +
                                    "\r\n",
                                    Encoding.Default);

                    List<실시간데이터> data = dic.Value.ToList();
                    foreach (var val in data)
                    {
                        string msg = string.Format("{0},{1},{2},{3}",
                                                    val.dt시간.ToString("yyyy-MM-dd HH:mm:ss"),
                                                    val.n현재가,
                                                    val.n고가,
                                                    val.n저가);

                        msg += "," +
                            val.s전일대비 + "," +
                            val.s등락률 + "," +
                            val.s매도호가 + "," +
                            val.s매수호가 + "," +
                            val.s누적거래량 + "," +
                            val.s누적거래대금 + "," +
                            val.s전일대비기호 + "," +
                            val.s전일거래량대비_계약 + "," +
                            val.s거래대금증감 + "," +
                            val.s전일거래량대비_비율 + "," +
                            val.s거래회전율 + "," +
                            val.s거래비용 + "," +
                            val.s상한가발생시간 + "," +
                            val.s하한가발생시간 + "," +
                            val.s체결량 + "," +
                            val.s체결강도;

                        File.AppendAllText(dir실시간데이터 + fileName, msg + "\r\n");
                    }
                }


                // 결과보고
                FormResult frmResult = new FormResult(_list매도종목);
                frmResult.ShowDialog();
            }

            if (DateTime.Now.TimeOfDay < TimeSpan.Parse(_장시작시간)
                || DateTime.Now.TimeOfDay > TimeSpan.Parse(_장마감시간))
            {
                return false;
            }

            if (DateTime.Now.TimeOfDay > TimeSpan.Parse(_장시작시간)
                && _b결과보고 == false)
            {
                _b결과보고 = true;
            }

            return true;
        }
        #endregion [공용 함수]


        #region [사용자 함수]
        private void 로그인()
        {
            if(axKHOpenAPI.CommConnect() == 0)
            {
                Logger(Log.로그창, "로그인창 열기 성공");
            }
            else
            {
                Logger(Log.로그창, "로그인창 열기 실패");
            }
        }

        private void 로그아웃()
        {
            DisconnectAllRealData();
            axKHOpenAPI.CommTerminate();
        }

        private void 조건식_가져오기()
        {
            axKHOpenAPI.GetConditionLoad();            
        }

        /// <summary>
        /// 매수주문
        /// </summary>
        private void f매수주문()
        {
            // 장시간 체크
            if (f장시간체크() == false) return;


            List<조건검색종목> listItem = _ConSearchItemList.ToList();
            foreach (조건검색종목 item in listItem)
            {
                if (item.b잔고매도) continue;
                if (item.n현재가 == 0) continue;                
                if (item.n현재가 == 0) continue;

                if (item.n매수주문단계 > 0)
                {
                    //경과시간이 지나면 매도주문 취소
                    if (item.n매수주문단계 == (int)주문단계.주문접수)
                    {
                        TimeSpan ts = DateTime.Now - item.dt매수주문시간;
                        if (ts.TotalMinutes > 5)
                        {
                            if (f주문취소(item.str종목코드, item.str주문번호, 주문구분.매수취소))
                            {
                                item.n매수주문단계 = 0;
                                f아이템삭제(item); // 삭제
                            }
                        }
                    }

                    continue;
                }

                int conIndex = f등록된조건옵션찾기(item.n조건번호);
                int n매수금액 = _ConRegistList[conIndex].n매수금액;

                if (item.n현재가 > n매수금액)
                {
                    f아이템삭제(item);
                    continue;
                }


                if (item.n편입매수시기강도 < CONST_편입매수시기강도_삭제)
                {
                    f아이템삭제(item); // 삭제
                    continue;
                }
                if (item.n편입매수시기강도 < CONST_편입매수시기강도_매수) continue;

                //int n중간가 = (item.n고가 + item.n저가) / 2;
                //if (n중간가 < item.n현재가) continue;


                int n매수주문금액 = item.n현재가;
                //매수조건 e매수조건 = _ConRegistList[conIndex].e매수조건;
                //switch (e매수조건)
                //{
                //    case 매수조건.P1아래: // 1% 아래
                //        break;
                //    case 매수조건.P2아래: // 2% 아래
                //        break;
                //    case 매수조건.P3아래: // 3% 아래
                //        break;
                //}

                int n매수수량 = n매수금액 / n매수주문금액;

                f지정가매수(item.str종목코드, n매수수량, n매수주문금액, ref item.n매수주문단계);
            }
        }

        /// <summary>
        /// 매도주문
        /// </summary>
        private void f매도주문()
        {
            // 장시간 체크
            if (f장시간체크() == false) return;


            List<조건검색종목> listItem = _ConSearchItemList.ToList();
            foreach (var item in listItem)
            {
                bool b매도주문 = false;                
                if (item.n매입가 == 0 || item.n현재가 == 0 || item.d손익율 == 0) continue;
                if (item.n매도주문단계 > 0)
                {
                    //경과시간이 지나면 매도주문 취소
                    if (item.n매도주문단계 == (int)주문단계.주문접수)
                    {
                        TimeSpan ts = DateTime.Now - item.dt매도주문시간;
                        if (ts.TotalMinutes > 5)
                        {
                            if (f주문취소(item.str종목코드, item.str주문번호, 주문구분.매도취소))
                            {
                                item.n매도주문단계 = 0;
                            }
                        }
                    }

                    continue;
                }

                if (item.b잔고매도)
                {
                    if (item.d손익율 >= item.d잔고수익률)
                    {
                        b매도주문 = true;
                    }

                    if (item.d잔고손절률 != 0 && item.d손익율 <= item.d잔고손절률)
                    {
                        b매도주문 = true;
                    }
                }
                else
                {
                    if (item.n매수주문단계 < (int)주문단계.모든체결완료) continue;
                    
                    int n등록된조건옵션인덱스 = f등록된조건옵션찾기(item.n조건번호);
                    double d수익률 = _ConRegistList[n등록된조건옵션인덱스].d수익률; // +
                    double d손절률 = _ConRegistList[n등록된조건옵션인덱스].d손절률; // -

                    if (item.d손익율 >= d수익률)
                    {
                        b매도주문 = true;
                    }

                    if (d손절률 != 0)
                    {
                        if (item.d손익율 <= d손절률)
                        {
                            b매도주문 = true;
                        }
                    }
                }

                if (b매도주문)
                {
                    f지정가매도(item.str종목코드, item.n보유수량, item.n현재가, ref item.n매도주문단계);
                }

            }
        }

        /// <summary>
        /// 주문취소
        /// </summary>
        private bool f주문취소(string str종목코드, string str주문번호, 주문구분 e주문구분)
        {
            // 계좌번호 입력 여부 확인
            if (_str계좌번호.Length != 10)
            {
                Logger(Log.로그창, "계좌번호 10자리를 입력해 주세요");
                return false;
            }

            int nRet = axKHOpenAPI.SendOrder("주문취소",
                                        _주문화면,
                                        _str계좌번호,
                                        (int)e주문구분,              // 매매구분 (3:매수취소, 4:매도취소)
                                        str종목코드,    // 종목코드
                                        0,      // 주문수량
                                        0,      // 주문가격 
                                        "00",           // 거래구분 (00:지정가, 03:시장가)
                                        str주문번호);           // 원주문 번호

            bool bRes = false;
            if (Error.IsError(nRet))
            {
                bRes = true;
                Logger(Log.로그창, "주문취소 전송 되었습니다");
            }
            else
            {
                Logger(Log.로그창, "주문취소 실패 하였습니다. [에러] : " + Error.GetErrorMessage());
            }

            Thread.Sleep(240);                  //  초당 5회 시간조절
            return bRes;
        }

        private void f손익율계산()
        {
            List<조건검색종목> listItem = _ConSearchItemList.ToList();
            foreach (조건검색종목 item in listItem)
            {
                if (item.n매입가 == 0 || item.n현재가 == 0) continue;

                // 신전투자일 경우 0.03
                double d수수료 = 0.03;
                if (_ServerGubun.Equals("1"))
                {
                    // 모의투자일 경우 0.7
                    d수수료 = 0.7;
                }

                double d세금 = 0.3;
                double d종목매입금액 = item.n매입가 * item.n보유수량;
                double d종목매도금액 = item.n현재가 * item.n보유수량;
                double d손익율 = (d종목매도금액 - d종목매입금액) / d종목매입금액 * 100 - d세금 - d수수료;

                item.d손익율 = d손익율;
            }
        }
            
        /// <summary>
        /// 20종목이상을 조회하려면 "계좌평가잔고내역" 연속조회를 구현
        /// </summary>
        /// <param name="prevNext"></param>
        /// <returns></returns>
        public int GetMyAccountState(int prevNext)
        {
            int rtnValue = 0;
            string strAccountNum = cbo_계좌정보.SelectedItem.ToString();
            
            // 계좌평가잔고내역요청 - opw00018 은 한번에 20개의 종목정보를 반환
            axKHOpenAPI.SetInputValue("계좌번호", strAccountNum);
            int nRet = axKHOpenAPI.CommRqData("계좌평가잔고내역요청", "OPW00018", prevNext, _잔고화면);
            if (!Error.IsError(nRet))
            {
                Logger(Log.로그창, "[계좌평가잔고내역요청] " + Error.GetErrorMessage());
                rtnValue = -1;
            }
            
            return rtnValue;
        }

        private bool f실시간시세등록(string strCode)
        {
            bool bRes = false;
            string str실시간등록타입 = "0";

            if (_b실시간등록타입 == false)
            {
                _b실시간등록타입 = true;
            }
            else
            {
                str실시간등록타입 = "1";
            }


            //public string s전일대비;        // 11
            //public string s등락률;         // 12
            //public string s매도호가;        // 27
            //public string s매수호가;        // 28
            //public string s누적거래량;       // 13
            //public string s누적거래대금;      // 14
            //public string s전일대비기호;      // 25
            //public string s전일거래량대비_계약;  // 26
            //public string s거래대금증감;          // 29
            //public string s전일거래량대비_비율;  // 30
            //public string s거래회전율;           // 31
            //public string s거래비용;            // 32
            //public string s상한가발생시간;     // 567
            //public string s하한가발생시간;     // 568
            //public string s체결량;             // 15
            //public string s체결강도;            // 228

            string strFidList = string.Format("{0};{1};{2};11;12;27;28;13;14;25;26;29;30;31;32;567;568;15;228",
                                                (int)RealType.enum주식시세.현재가,
                                                (int)RealType.enum주식시세.고가,
                                                (int)RealType.enum주식시세.저가);

            int nRet = axKHOpenAPI.SetRealReg(_실시간화면, strCode, strFidList, str실시간등록타입); // 실시간 시세등록
            if (Error.IsError(nRet))
            {
                Logger(Log.로그창, string.Format("[{0}] 실시간시세등록 성공", strCode));
                bRes = true;
            }
            else
            {
                Logger(Log.로그창, string.Format("[{0}] 실시간시세등록 실패 [에러] : {1}", strCode, Error.GetErrorMessage()));
                bRes = false;
            }

            return bRes;
        }

        private bool f실시간조건검색등록(string str조건명, int n조건번호)
        {
            bool bRes = false;

            int nRet = axKHOpenAPI.SendCondition(_조건검색화면, str조건명, n조건번호, 1);
            if (nRet == 1)
            {
                Logger(Log.로그창, string.Format("[{0}] 실시간조건검색등록 성공", str조건명));
                bRes = true;
            }
            else
            {
                Logger(Log.로그창, string.Format("[{0}] 실시간조건검색등록 실패 [에러] : ", str조건명, nRet));
                bRes = false;
            }

            return bRes;
        }

        private void f아이템삭제(조건검색종목 item)
        {
            _ConSearchItemList.Remove(item);

            int rowIndex = f테이블아이템찾기(_dt매수매도아이템리스트, item.str종목코드);
            if (rowIndex != -1)
            {
                //_dt매수매도아이템리스트.Rows.RemoveAt(rowIndex);
                grd_아이템리스트.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Gray;
            }
        }
        #endregion [사용자 함수]



        #region [키움증권 이벤트]
        private void f지정가매수(string str종목코드, int n주문수량, int n주문가, ref int n매수주문단계)
        {
            // 계좌번호 입력 여부 확인
            if (_str계좌번호.Length != 10)
            {
                Logger(Log.로그창, "계좌번호 10자리를 입력해 주세요");
                return;
            }

            int nRet = axKHOpenAPI.SendOrder("자동매수주문",
                                        _주문화면,
                                        _str계좌번호,
                                        1,              // 매매구분 (신규매수)
                                        str종목코드,    // 종목코드
                                        n주문수량,         // 주문수량
                                        n주문가,         // 주문가격 
                                        "00",    // "03":시장가, "00":지정가
                                        "0");           // 원주문 번호

            if (Error.IsError(nRet))
            {
                n매수주문단계 = (int)주문단계.주문요청;
                Logger(Log.로그창, "지정가매수 전송 되었습니다");
            }
            else
            {
                Logger(Log.로그창, "지정가매수 실패 하였습니다. [에러] : " + Error.GetErrorMessage());
            }

            Thread.Sleep(240);                  //  초당 5회 시간조절
        }

        private void f지정가매도(string str종목코드, int n주문수량, int n주문가, ref int n매도주문단계)
        {
            // 계좌번호 입력 여부 확인
            if (_str계좌번호.Length != 10)
            {
                Logger(Log.로그창, "계좌번호 10자리를 입력해 주세요");
                return;
            }

            int nRet = axKHOpenAPI.SendOrder("지정가매도주문",
                                        _주문화면,
                                        _str계좌번호,
                                        2,              // 매매구분 (신규매도)
                                        str종목코드,    // 종목코드
                                        n주문수량,      // 주문수량
                                        n주문가,        // 주문가격 
                                        "00",    // "03":시장가, "00":지정가
                                        "0");           // 원주문 번호

            if (Error.IsError(nRet))
            {
                n매도주문단계 = (int)주문단계.주문요청;
                Logger(Log.로그창, "지정가매도 전송 되었습니다");
            }
            else
            {
                Logger(Log.로그창, "지정가매도 실패 하였습니다. [에러] : " + Error.GetErrorMessage());
            }

            Thread.Sleep(240);                  //  초당 5회 시간조절
        }

        private void axKHOpenAPI_OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (Error.IsError(e.nErrCode))
            {
                _ServerGubun = axKHOpenAPI.GetLoginInfo("GetServerGubun");

                string[] arr계좌 = axKHOpenAPI.GetLoginInfo("ACCNO").Split(';');
                cbo_계좌정보.Items.AddRange(arr계좌);
                if(cbo_계좌정보.Items.Count > 0)
                {
                    cbo_계좌정보.SelectedIndex = 0;
                }

                조건식_가져오기();

                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                Logger(Log.로그창, "[로그인 처리결과] " + Error.GetErrorMessage());
            }
        }

        private void axKHOpenAPI_OnReceiveMsg(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {
            Logger(Log.로그창, "화면번호:{0} | RQName:{1} | TRCode:{2} | 메세지:{3}", e.sScrNo, e.sRQName, e.sTrCode, e.sMsg);
        }

        #region [TR 관련 이벤트]
        private void axKHOpenAPI_OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            // OPT1001 : 주식기본정보요청
            if (e.sRQName == "주식기본정보요청")
            {
                string srt종목코드 = axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "종목코드").Trim();
                int n현재가 = f가격변환(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "현재가").Trim());


                //string str종목명 = axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, 0, "종목명").Trim();
                //string str현재가 = changeFormat(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "현재가").Trim());

                //Logger(Log.로그창, "{0} | 현재가:{1} | 등락율:{2} | 거래량:{3:N0} ",
                //       str종목명, str현재가,
                //       axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "등락율").Trim(),
                //       Int32.Parse(axKHOpenAPI.GetCommData(e.sTrCode, "", 0, "거래량").Trim()));
            }
            // 예수금상세현황요청 : OPW00001
            else if (e.sRQName == "예수금상세현황요청")
            {
                string data = axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, 0, "예수금").Trim();
                int d = Convert.ToInt32(data);
                string str예수금 = string.Format("{0:N0}", d);
                txt_잔액.Text = str예수금;
            }
            // 계좌평가잔고내역요청 : OPW00018
            else if (e.sRQName == "계좌평가잔고내역요청")
            {
                // 보유 종목 정보
                int cnt = axKHOpenAPI.GetRepeatCnt(e.sTrCode, e.sRQName);
                int nPrevNext = 0;

                if (!string.IsNullOrEmpty(e.sPrevNext))
                {
                    //연속데이터 존재여부
                    nPrevNext = Int32.Parse(e.sPrevNext);
                }

                List<string> itemList = new List<string>(); // grd_잔고.Columns.Count self.GetTableColumnList(self.ui.table_HaveItemList)
                for (int i=0; i<grd_잔고.Columns.Count; i++)
                {
                    string strColumnName = grd_잔고.Columns[i].HeaderText;
                    itemList.Add(strColumnName);
                }

                for (int i=0; i<cnt; i++)
                {
                    List<object> data = new List<object>();
                    foreach (string item in itemList)
                    {
                        string strValue = axKHOpenAPI.GetCommData(e.sTrCode, e.sRQName, i, item).Trim();

                        if (item.StartsWith("수익률"))
                        {
                            strValue = changeFormat(strValue, 1);
                        }
                        else if (item.Equals("종목번호"))
                        {
                            strValue = f종목코드변환(strValue);
                        }
                        else if (!item.Equals("종목명"))
                        {
                            strValue = changeFormat(strValue);
                        }

                        data.Add(strValue);
                    }

                    grd_잔고.Rows.Add(data.ToArray());
                }

                grd_잔고.AutoResizeColumns();
                grd_잔고.AutoResizeRows();

                // 연속조회가 있을 경우 다음 화면 요청하여 출력
                if (nPrevNext == 2)
                {
                    GetMyAccountState(nPrevNext);
                }
                else
                {
                    //nPrevNext = 0;                     // 이부분에 작성할 코드를 잘 모르겠음 
                }

            }
        }

        private void axKHOpenAPI_OnReceiveRealData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            string str종목코드 = f종목코드변환(e.sRealKey);

            int findIndex = f조건검색종목찾기(str종목코드);
            if (findIndex == -1) return;

            //Logger(Log.로그창, "종목코드 : {0} | RealType : {1} | RealData : {2}", e.sRealKey, e.sRealType, e.sRealData);

            if (e.sRealType == "주식시세" || e.sRealType == "주식체결" || e.sRealType == "주식예상체결")
            {
                string str현재가 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.현재가).Trim();
                string str고가 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.고가).Trim();
                string str저가 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.저가).Trim();

                int n현재가 = f가격변환(str현재가);
                int n고가 = f가격변환(str고가);
                int n저가 = f가격변환(str저가);

                if (_ConSearchItemList[findIndex].n현재가 > 0)
                {
                    if (n현재가 > _ConSearchItemList[findIndex].n현재가)
                    {
                        _ConSearchItemList[findIndex].n편입매수시기강도++;
                    }
                    else if (n현재가 < _ConSearchItemList[findIndex].n현재가)
                    {
                        _ConSearchItemList[findIndex].n편입매수시기강도--;
                    }
                }


                _ConSearchItemList[findIndex].n현재가 = n현재가;
                _ConSearchItemList[findIndex].n고가 = n고가;
                _ConSearchItemList[findIndex].n저가 = n저가;


                // 실시간 데이터 로그 남기기
                실시간데이터 C데이터 = new 실시간데이터();
                C데이터.dt시간 = DateTime.Now;
                C데이터.str종목명 = _ConSearchItemList[findIndex].str종목명;
                C데이터.n현재가 = n현재가;
                C데이터.n고가 = n고가;
                C데이터.n저가 = n저가;

                C데이터.s전일대비 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.전일대비).Trim();        // 11
                C데이터.s등락률 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.등락율).Trim();         // 12
                C데이터.s매도호가 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.최우선매도호가).Trim();        // 27
                C데이터.s매수호가 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.최우선매수호가).Trim();        // 28
                C데이터.s누적거래량 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.누적거래량).Trim();       // 13
                C데이터.s누적거래대금 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.누적거래대금).Trim();      // 14
                C데이터.s전일대비기호 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.전일대비기호).Trim();      // 25
                C데이터.s전일거래량대비_계약 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.전일거래량대비_계약).Trim();  // 26
                C데이터.s거래대금증감 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.거래대금증감).Trim();          // 29
                C데이터.s전일거래량대비_비율 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.전일거래량대비_비율).Trim();  // 30
                C데이터.s거래회전율 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.거래회전율).Trim();           // 31
                C데이터.s거래비용 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.거래비용).Trim();            // 32
                C데이터.s상한가발생시간 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.상한가발생시간).Trim();     // 567
                C데이터.s하한가발생시간 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식시세.하한가발생시간).Trim();     // 568
                C데이터.s체결량 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식체결.체결량).Trim();             // 15
                C데이터.s체결강도 = axKHOpenAPI.GetCommRealData(e.sRealType, (int)RealType.enum주식체결.체결강도).Trim();            // 228
                
                
                if (_dic실시간데이터.ContainsKey(str종목코드) == false)
                {
                    _dic실시간데이터.Add(str종목코드, new List<실시간데이터>());
                }
                _dic실시간데이터[str종목코드].Add(C데이터);

            }
        }
        #endregion [TR 관련 이벤트]


        #region [주문 관련 이벤트]
        private void axKHOpenAPI_OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            if (e.sGubun == "0")
            {
                string str종목코드 = f종목코드변환(axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.종목코드).Trim());
                int findIndex = f조건검색종목찾기(str종목코드);

                if (findIndex == -1) return;

                string str주문상태 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.주문상태).Trim();
                string str매매구분 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.주문구분).Trim(); // +매수, -매도

                if (str주문상태.Equals("접수"))
                {                    
                    string str주문번호 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.주문번호).Trim();
                    string str주문가격 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.주문가격).Trim();
                    string str주문수량 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.주문수량).Trim();
                    
                    _ConSearchItemList[findIndex].str주문번호 = str주문번호;

                    if (str매매구분.EndsWith("매수"))
                    {
                        _ConSearchItemList[findIndex].dt매수주문시간 = DateTime.Now;
                        _ConSearchItemList[findIndex].n매수주문단계 = (int)주문단계.주문접수;
                    }
                    else if (str매매구분.EndsWith("매도"))
                    {
                        _ConSearchItemList[findIndex].dt매도주문시간 = DateTime.Now;
                        _ConSearchItemList[findIndex].n매도주문단계 = (int)주문단계.주문접수;
                    }

                    Logger(Log.체결내역, "[접수] [{0}] [{1}] [가격: {2}원] [수량: {3}주]",
                                    str매매구분,
                                    axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.종목명).Trim(),
                                    str주문가격,
                                    str주문수량);
                }
                else if (str주문상태.Equals("체결"))
                {
                    string str체결가격 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.체결가).Trim();
                    string str체결수량 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.체결량).Trim();
                    int n체결가격 = f가격변환(str체결가격);
                    int n체결수량 = f가격변환(str체결수량);

                    // 취소시에도 체결정보가 들어오기 때문에
                    if (n체결가격 == 0 && n체결수량 == 0) return;


                    if (str매매구분.EndsWith("매수"))
                    {
                        _ConSearchItemList[findIndex].n매입가 = n체결가격;
                        _ConSearchItemList[findIndex].n보유수량 = n체결수량;
                        _ConSearchItemList[findIndex].dt매수체결시간 = DateTime.Now;

                        _ConSearchItemList[findIndex].n매수주문단계 = (int)주문단계.모든체결완료;
                    }
                    else if (str매매구분.EndsWith("매도"))
                    {
                        int index매도종목 = _list매도종목.FindIndex(x => x.str종목코드 == str종목코드);
                        if (index매도종목 == -1)
                        {
                            string str종목명 = axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.종목명).Trim();

                            매도종목 sell = new 매도종목();
                            sell.str편입시간 = _ConSearchItemList[findIndex].str시간;
                            sell.str종목코드 = str종목코드;
                            sell.str종목명 = str종목명;
                            sell.n매수가 = _ConSearchItemList[findIndex].n매입가;
                            sell.n매도가 = n체결가격;
                            sell.n매도수량 = n체결수량;
                            //sell.n매도체결카운트 = 1;

                            _list매도종목.Add(sell);
                        }
                        else
                        {
                            _list매도종목[index매도종목].n매도수량 = n체결수량;
                            _list매도종목[index매도종목].n매도가 = n체결가격;

                            //_list매도종목[index매도종목].n매도체결카운트++;
                            //_list매도종목[index매도종목].d손익율 = (_list매도종목[index매도종목].d손익율 + _ConSearchItemList[findIndex].d손익율) / _list매도종목[index매도종목].n매도체결카운트;
                        }

                        int n보유수량 = _ConSearchItemList[findIndex].n보유수량;
                        if (n보유수량 == n체결수량)
                        {
                            axKHOpenAPI.SetRealRemove(_실시간화면, str종목코드); // 실시간 시세해지
                            f아이템삭제(_ConSearchItemList[findIndex]); // 삭제
                        }
                    }

                    Logger(Log.체결내역, "[체결] [{0}] [{1}] [가격: {2}원] [수량: {3}주]",
                                    str매매구분,
                                    axKHOpenAPI.GetChejanData((int)FidList.CHEJAN.종목명).Trim(),
                                    n체결가격,
                                    n체결수량);
                }
            }
            else if (e.sGubun == "1")
            {
                Logger(Log.로그창, "구분 : 잔고통보");
            }
            else if (e.sGubun == "3")
            {
                Logger(Log.로그창, "구분 : 특이신호");
            }
        }
        #endregion [주문 관련 이벤트]


        #region [조건식 관련 이벤트]
        private void axKHOpenAPI_OnReceiveConditionVer(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
        {
            _ConditionList.Clear();
            string strConList = axKHOpenAPI.GetConditionNameList().Trim();

            Logger(Log.로그창, strConList);

            // 분리된 문자 배열 저장
            string[] spConList = strConList.Split(';');

            // ComboBox 출력
            for (int i = 0; i < spConList.Length; i++)
            {
                if (spConList[i].Trim().Length >= 2)
                {
                    string[] spCon = spConList[i].Split('^');
                    int nIndex = Int32.Parse(spCon[0]);
                    string strConditionName = spCon[1];

                    ConditionList ConData = new ConditionList();
                    ConData.nIndex = nIndex;
                    ConData.strConditionName = strConditionName;
                    _ConditionList.Add(ConData);

                    cbo_조건식리스트.Items.Add(string.Format("[{0}]{1}", nIndex, strConditionName));
                }
            }

            if (cbo_조건식리스트.Items.Count > 0)
            {
                cbo_조건식리스트.SelectedIndex = 0;
            }
        }

        // =====================================================<< 조건식 TR 메세지 수신부 >> ================================================//
        // 수신된 종목코드 문자열 분리
        // 최초 조건검색후 종목코드 수신하는곳으로 이후에는 OnReceiveRealCondition 에서 실시간 수신됨 
        // ===================================================================================================================================//
        private void axKHOpenAPI_OnReceiveTrCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
        {
            Logger(Log.로그창, "===== 조건식 조회  =====");
            Logger(Log.로그창, "[화면번호] : " + e.sScrNo);
            Logger(Log.로그창, "[종목리스트] : " + e.strCodeList);
            Logger(Log.로그창, "[조건명] : " + e.strConditionName);
            Logger(Log.로그창, "[조건명 인덱스] : " + e.nIndex);

            string[] strCodeList = e.strCodeList.Split(';');
            int cnt종목코드 = strCodeList.Length - 1;

            if (strCodeList.Length > 1)
            {
                for (int i = 0; i < cnt종목코드; i++)
                {
                    string str시간 = DateTime.Now.ToString("HH:mm:ss");
                    string str종목코드 = f종목코드변환(strCodeList[i]);
                    string str종목명 = axKHOpenAPI.GetMasterCodeName(strCodeList[i]);
                    int n조건번호 = e.nIndex;
                    string str조건명 = e.strConditionName;

                    조건검색종목 ConSearchItem = new 조건검색종목();
                    ConSearchItem.str시간 = str시간;
                    ConSearchItem.str종목코드 = str종목코드;
                    ConSearchItem.str종목명 = str종목명;
                    ConSearchItem.n조건번호 = n조건번호;
                    ConSearchItem.str조건명 = str조건명;

                    f종목편입업데이트(ConSearchItem, "I");
                }
            }
        }

        // =================================================<<조건조회 실시간 편입/이탈 정보 업데이트>>========================================//
        // * 자동주문 로직**
        // 조건조회 실시간 편입/이탈 정보 업데이트하여 데이터그리드뷰에 갱신하기
        //  
        // ====================================================================================================================================//
        private void axKHOpenAPI_OnReceiveRealCondition(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
        {
            //Logger(Log.로그창, "======= 조건조회 실시간 편입/이탈 =======");
            //Logger(Log.로그창, "[종목코드] : " + e.sTrCode);
            //Logger(Log.로그창, "[실시간타입] : " + e.strType);
            //Logger(Log.로그창, "[조건명] : " + e.strConditionName);
            //Logger(Log.로그창, "[조건명 인덱스] : " + e.strConditionIndex);

            string str시간 = DateTime.Now.ToString("HH:mm:ss");
            string str종목코드 = f종목코드변환(e.sTrCode);
            string str종목명 = axKHOpenAPI.GetMasterCodeName(e.sTrCode);
            int n조건번호 = Convert.ToInt32(e.strConditionIndex);
            string str조건명 = e.strConditionName;

            조건검색종목 ConSearchItem = new 조건검색종목();
            ConSearchItem.str시간 = str시간;
            ConSearchItem.str종목코드 = str종목코드;
            ConSearchItem.str종목명 = str종목명;
            ConSearchItem.n조건번호 = n조건번호;
            ConSearchItem.str조건명 = str조건명;

            f종목편입업데이트(ConSearchItem, e.strType);
        }
        #endregion [조건식 관련 이벤트]

        #endregion [키움증권 이벤트]



        #region [실시간 편입종목을 업데이트]
        // =================================================
        // 실시간 편입종목을 업데이트
        // =================================================
        private void f종목편입업데이트(조건검색종목 ConSearchItem, string str실시간타입)
        {   
            int findIndex = -1;

            switch (str실시간타입)
            {
                case "I":
                    // 기존 등록 종목 유무
                    findIndex = f조건검색종목찾기(ConSearchItem.str종목코드);
                    if (findIndex != -1) return;


                    _ConSearchItemList.Add(ConSearchItem);
                    f실시간시세등록(ConSearchItem.str종목코드);

                    _dt조건검색종목리스트.Rows.Add(ConSearchItem.str시간,
                                                    ConSearchItem.str종목코드,
                                                    ConSearchItem.str종목명,
                                                    ConSearchItem.str조건명);
                    break;
                case "D":
                    int rowIndex = f테이블아이템찾기(_dt조건검색종목리스트, ConSearchItem.str종목코드);
                    if (rowIndex != -1)
                    {
                        _dt조건검색종목리스트.Rows.RemoveAt(rowIndex);
                    }
                    break;
            }
        }
        #endregion [실시간 편입종목을 업데이트]



        #region [실시간 매수 매도]
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_bBackLoop)
            {
                if (_ProcDelay == 0)
                {
                    _ProcDelay = 100;

                    if (DateTime.Now.TimeOfDay > TimeSpan.Parse(_장시작시간)
                        && _list실시간등록.Count > 0)
                    {
                        List<실시간등록> listReal = _list실시간등록.ToList();

                        foreach (var d in listReal)
                        {
                            switch (d.e실시간종류)
                            {
                                case 실시간종류.조건검색:
                                    f실시간조건검색등록(d.strParam1, int.Parse(d.strParam2));
                                    break;
                                case 실시간종류.주식시세:
                                    f실시간시세등록(d.strParam1);
                                    break;
                            }
                        }

                        _list실시간등록.Clear();
                    }


                    f매수주문();
                    f손익율계산();
                    f매도주문();
                }

                if (_GridViewDelay == 0)
                {
                    _GridViewDelay = 1000;
                    f아이템리스트to테이블();
                }

            }            
        }

        #endregion [실시간 매수 매도]


        #region [상태바 시계]
        private void timer_Status_Tick(object sender, EventArgs e)
        {
            statusLabel_DateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        #endregion [상태바 시계]



        #region [XML]
        private void Xml파일()
        {

        }
        #endregion [XML]



    }


    public class ConditionList
    {
        public string strConditionName;
        public int nIndex;
    }

    public class 등록된조건옵션
    {
        public int n조건번호;
        public string str조건명;
        public 매수조건 e매수조건;
        public int n매수금액;
        public double d수익률;
        public double d손절률;
        public bool b실시간;
    }

    public class 조건검색종목
    {
        public string str시간;
        public string str종목코드;
        public string str종목명;
        public int n조건번호;
        public string str조건명;

        public int n현재가;
        public int n고가;
        public int n저가;

        public int n보유수량;
        public int n매입가;
        public double d손익율;

        public bool b잔고매도;
        public double d잔고수익률;
        public double d잔고손절률;

        public int n매수주문단계;
        public int n매도주문단계;

        public string str주문번호;
        public DateTime dt매수주문시간;
        public DateTime dt매도주문시간;

        public int n편입매수시기강도;
        public DateTime dt매수체결시간;
    }

    public class 매도종목
    {
        public string str편입시간;
        public string str종목코드;
        public string str종목명;
        public int n매수가;
        public int n매도가;
        public int n매도수량;

        public int n매도체결카운트;
    }

    public class 실시간등록
    {
        public string strParam1;
        public string strParam2;

        public 실시간종류 e실시간종류;

        public 실시간등록(string param1, string param2, 실시간종류 param4)
        {
            this.strParam1 = param1;
            this.strParam2 = param2;
            this.e실시간종류 = param4;
        }
    }


    public class 실시간데이터
    {
        public DateTime dt시간;
        public string str종목명;
        public int n현재가;        // 10
        public int n고가;             //
        public int n저가;

        public string s전일대비;        // 11
        public string s등락률;         // 12
        public string s매도호가;        // 27
        public string s매수호가;        // 28
        public string s누적거래량;       // 13
        public string s누적거래대금;      // 14
        public string s전일대비기호;      // 25
        public string s전일거래량대비_계약;  // 26
        public string s거래대금증감;          // 29
        public string s전일거래량대비_비율;  // 30
        public string s거래회전율;           // 31
        public string s거래비용;            // 32
        public string s상한가발생시간;     // 567
        public string s하한가발생시간;     // 568
        public string s체결량;             // 15
        public string s체결강도;            // 228
    }


}
