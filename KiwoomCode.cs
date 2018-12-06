using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiwoomCode
{
    public enum Log
    {
        체결내역,     // 체결내역 출력
        로그창,       // 디버그 로그 출력
    }    

    class KOAErrorCode
    {
        public const int OP_ERR_NONE = 0;     //"정상처리"
        public const int OP_ERR_LOGIN = -100;  //"사용자정보교환에 실패하였습니다. 잠시후 다시 시작하여 주십시오."
        public const int OP_ERR_CONNECT = -101;  //"서버 접속 실패"
        public const int OP_ERR_VERSION = -102;  //"버전처리가 실패하였습니다.
        public const int OP_ERR_SISE_OVERFLOW = -200;  //”시세조회 과부하”
        public const int OP_ERR_RQ_STRUCT_FAIL = -201;  //”REQUEST_INPUT_st Failed”
        public const int OP_ERR_RQ_STRING_FAIL = -202;  //”요청 전문 작성 실패”
        public const int OP_ERR_ORD_WRONG_INPUT = -300;  //”입력값 오류”
        public const int OP_ERR_ORD_WRONG_ACCNO = -301;  //”계좌비밀번호를 입력하십시오.”
        public const int OP_ERR_OTHER_ACC_USE = -302;  //”타인계좌는 사용할 수 없습니다.
        public const int OP_ERR_MIS_2BILL_EXC = -303;  //”주문가격이 20억원을 초과합니다.”
        public const int OP_ERR_MIS_5BILL_EXC = -304;  //”주문가격은 50억원을 초과할 수 없습니다.”
        public const int OP_ERR_MIS_1PER_EXC = -305;  //”주문수량이 총발행주수의 1%를 초과합니다.”
        public const int OP_ERR_MID_3PER_EXC = -306;  //”주문수량은 총발행주수의 3%를 초과할 수 없습니다.”
    }
    
    public class FidList
    {
        public enum CHEJAN
        {
            계좌번호 = 9201,
            주문번호 = 9203,
            관리자사번 = 9205,
            종목코드 = 9001,
            주문업무분류 = 912,
            주문상태 = 913,
            종목명 = 302,
            주문수량 = 900,
            주문가격 = 901,
            미체결수량 = 902,
            체결누계금액 = 903,
            원주문번호 = 904,
            주문구분 = 905,
            매매구분 = 906,
            매도수구분 = 907,
            주문_체결시간 = 908,
            체결번호 = 909,
            체결가 = 910,
            체결량 = 911,
            현재가 = 10,
            최우선_매도호가 = 27,
            최우선_매수호가 = 28,
            단위체결가 = 914,
            단위체결량 = 915,
            당일매매수수료 = 938,
            당일매매세금 = 939,
            거부사유 = 919,
            화면번호 = 920,
            신용구분 = 917,
            대출일 = 916,
            보유수량 = 930,
            매입단가 = 931,
            총매입가 = 932,
            주문가능수량 = 933,
            당일순매수수량 = 945,
            매도_매수구분 = 946,
            당일총매도손일 = 950,
            예수금 = 951,
            기준가 = 307,
            손익율 = 8019,
            신용금액 = 957,
            신용이자 = 958,
            담보대출수량 = 959,
            만기일 = 918,
            당일실현손익_유가 = 990,
            당일신현손익률_유가 = 991,
            당일실현손익_신용 = 992,
            당일실현손익률_신용 = 993,
            파생상품거래단위 = 397,
            상한가 = 305,
            하한가 = 306
        }
    }
    

    public class RealType
    {
        public enum enum주식시세
        {
            현재가 = 10,
            전일대비 = 11,
            등락율 = 12,
            최우선매도호가 = 27,
            최우선매수호가 = 28,
            누적거래량 = 13,
            누적거래대금 = 14,
            시가 = 16,
            고가 = 17,
            저가 = 18,
            전일대비기호 = 25,
            전일거래량대비_계약 = 26,
            거래대금증감 = 29,
            전일거래량대비_비율 = 30,
            거래회전율 = 31,
            거래비용 = 32,
            시가총액 = 311, //(억)
            상한가발생시간 = 567,
            하한가발생시간 = 568
        }

        public enum enum주식체결
        {
            체결시간 = 20, //(HHMMSS)
            체결가 = 10,
            전일대비 = 11,
            등락율 = 12,
            최우선매도호가 = 27,
            최우선매수호가 = 28,
            체결량 = 15,
            누적체결량 = 13,
            누적거래대금 = 14,
            시가 = 16,
            고가 = 17,
            저가 = 18,
            전일대비기호 = 25,
            전일거래량대비 = 26, //(계약,주)
            거래대금증감 = 29,
            전일거래량대비비율 = 30,
            거래회전율 = 31,
            거래비용 = 32,
            체결강도 = 228,
            시가총액 = 311, //(억)
            장구분 = 290,
            KO접근도 = 691
        }

        public enum enum주식호가잔량
        {
            호가시간 = 21,
            매도호가1 = 41,
            매도호가수량1 = 61,
            매도호가직전대비1 = 81,
            매수호가1 = 51,
            매수호가수량1 = 71,
            매수호가직전대비1 = 91,
            매도호가2 = 42,
            매도호가수량2 = 62,
            매도호가직전대비2 = 82,
            매수호가2 = 52,
            매수호가수량2 = 72,
            매수호가직전대비2 = 92,
            매도호가3 = 43,
            매도호가수량3 = 63,
            매도호가직전대비3 = 83,
            매수호가3 = 53,
            매수호가수량3 = 73,
            매수호가직전대비3 = 93,
            매도호가4 = 44,
            매도호가수량4 = 64,
            매도호가직전대비4 = 84,
            매수호가4 = 54,
            매수호가수량4 = 74,
            매수호가직전대비4 = 94,
            매도호가5 = 45,
            매도호가수량5 = 65,
            매도호가직전대비5 = 85,
            매수호가5 = 55,
            매수호가수량5 = 75,
            매수호가직전대비5 = 95,
            매도호가6 = 46,
            매도호가수량6 = 66,
            매도호가직전대비6 = 86,
            매수호가6 = 56,
            매수호가수량6 = 76,
            매수호가직전대비6 = 96,
            매도호가7 = 47,
            매도호가수량7 = 67,
            매도호가직전대비7 = 87,
            매수호가7 = 57,
            매수호가수량7 = 77,
            매수호가직전대비7 = 97,
            매도호가8 = 48,
            매도호가수량8 = 68,
            매도호가직전대비8 = 88,
            매수호가8 = 58,
            매수호가수량8 = 78,
            매수호가직전대비8 = 98,
            매도호가9 = 49,
            매도호가수량9 = 69,
            매도호가직전대비9 = 89,
            매수호가9 = 59,
            매수호가수량9 = 79,
            매수호가직전대비9 = 99,
            매도호가10 = 50,
            매도호가수량10 = 70,
            매도호가직전대비10 = 90,
            매수호가10 = 60,
            매수호가수량10 = 80,
            매수호가직전대비10 = 100,
            매도호가총잔량 = 121,
            매도호가총잔량직전대비 = 122,
            매수호가총잔량 = 125,
            매수호가총잔량직전대비 = 126,
            예상체결가 = 23,
            예상체결수량 = 24,
            순매수잔량 = 128, //(총매수잔량-총매도잔량)
            매수비율 = 129,
            순매도잔량 = 138, //(총매도잔량-총매수잔량)
            매도비율 = 139,
            예상체결가전일종가대비 = 200,
            예상체결가전일종가대비등락율 = 201,
            예상체결가전일종가대비기호 = 238,
            //예상체결가 = 291,
            예상체결량 = 292,
            예상체결가전일대비기호 = 293,
            예상체결가전일대비 = 294,
            예상체결가전일대비등락율 = 295,
            누적거래량 = 13,
            전일거래량대비예상체결률 = 299,
            장운영구분 = 215
        }

        public enum enum장시작시간
        {
            장운영구분 = 215, //(0:장시작전, 2:장종료전, 3:장시작, 4,8:장종료, 9:장마감)
            시간 = 20, //(HHMMSS)
            장시작예상잔여시간 = 214
        }

        public enum enum업종지수
        {
            체결시간 = 20,
            현재가 = 10,
            전일대비 = 11,
            등락율 = 12,
            거래량 = 15,
            누적거래량 = 13,
            누적거래대금 = 14,
            시가 = 16,
            고가 = 17,
            저가 = 18,
            전일대비기호 = 25,
            전일거래량대비 = 26 //(계약, 주)
        }

        public enum enum업종등락
        {
            체결시간 = 20,
            상승종목수 = 252,
            상한종목수 = 251,
            보합종목수 = 253,
            하락종목수 = 255,
            하한종목수 = 254,
            누적거래량 = 13,
            누적거래대금 = 14,
            현재가 = 10,
            전일대비 = 11,
            등락율 = 12,
            거래형성종목수 = 256,
            거래형성비율 = 257,
            전일대비기호 = 25
        }

        public enum enum주문체결
        {
            계좌번호 = 9201,
            주문번호 = 9203,
            관리자사번 = 9205,
            종목코드 = 9001,
            주문분류 = 912, //(jj:주식주문)
            주문상태 = 913, //(10:원주문, 11:정정주문, 12:취소주문, 20:주문확인, 21:정정확인, 22:취소확인, 90,92:주문거부)
            종목명 = 302,
            주문수량 = 900,
            주문가격 = 901,
            미체결수량 = 902,
            체결누계금액 = 903,
            원주문번호 = 904,
            주문구분 = 905, //(+:현금매수, -:현금매도)
            매매구분 = 906, //(보통, 시장가등)
            매도수구분 = 907, //(1:매도, 2:매수)
            체결시간 = 908, //(HHMMSS)
            체결번호 = 909,
            체결가 = 910,
            체결량 = 911,
            //체결가 = 10,
            최우선매도호가 = 27,
            최우선매수호가 = 28,
            단위체결가 = 914,
            단위체결량 = 915,
            당일매매수수료 = 938,
            당일매매세금 = 939
        }

        public enum enum잔고
        {
            계좌번호 = 9201,
            종목코드 = 9001,
            종목명 = 302,
            현재가 = 10,
            보유수량 = 930,
            매입단가 = 931,
            총매입가 = 932,
            주문가능수량 = 933,
            당일순매수량 = 945,
            매도매수구분 = 946,
            당일총매도손익 = 950,
            예수금 = 951,
            최우선매도호가 = 27,
            최우선매수호가 = 28,
            기준가 = 307,
            손익율 = 8019
        }

        public enum enum주식시간외호가
        {
            호가시간 = 21, //(HHMMSS)
            시간외매도호가총잔량 = 131,
            시간외매도호가총잔량직전대비 = 132,
            시간외매수호가총잔량 = 135,
            시간외매수호가총잔량직전대비 = 136
        }
    }


    public class KOACode
    {

        /// <summary>
        /// 주문코드 클래스
        /// </summary>
        public struct OrderType
        {
            private string Name;
            private int Code;

            public OrderType(int nCode, string strName)
            {
                this.Name = strName;
                this.Code = nCode;
            }

            public string name
            {
                get
                {
                    return this.Name;
                }
            }

            public int code
            {
                get
                {
                    return this.Code;
                }
            }
        }

        public readonly static OrderType[] orderType = new OrderType[6];


        /// <summary>
        /// 호가구분 클래스
        /// </summary>
        public struct HogaGb
        {
            private string Name;
            private string Code;

            public HogaGb(string strCode, string strName)
            {
                this.Code = strCode;
                this.Name = strName;
            }

            public string name
            {
                get
                {
                    return this.Name;
                }
            }

            public string code
            {
                get
                {
                    return this.Code;
                }
            }
        }

        public readonly static HogaGb[] hogaGb = new HogaGb[13];

        public struct MarketCode
        {
            private string Name;
            private string Code;

            public MarketCode(string strCode, string strName)
            {
                this.Code = strCode;
                this.Name = strName;
            }

            public string name
            {
                get
                {
                    return this.Name;
                }
            }

            public string code
            {
                get
                {
                    return this.Code;
                }
            }
        }

        //public readonly static MarketCode[] marketCode = new MarketCode[9];

        static KOACode()
        {
            // 주문타입 설정
            orderType[0] = new OrderType(1, "신규매수");
            orderType[1] = new OrderType(2, "신규매도");
            orderType[2] = new OrderType(3, "매수취소");
            orderType[3] = new OrderType(4, "매도취소");
            //orderType[4] = new OrderType(5, "매수정정");
            //orderType[5] = new OrderType(6, "매도정정");

            // 호가타입 설정
            hogaGb[0] = new HogaGb("00", "지정가");
            hogaGb[1] = new HogaGb("03", "시장가");
            //hogaGb[2] = new HogaGb("05", "조건부지정가");
            //hogaGb[3] = new HogaGb("06", "최유리지정가");
            //hogaGb[4] = new HogaGb("07", "최우선지정가");
            //hogaGb[5] = new HogaGb("10", "지정가IOC");
            //hogaGb[6] = new HogaGb("13", "시장가IOC");
            //hogaGb[7] = new HogaGb("16", "최유리IOC");
            //hogaGb[8] = new HogaGb("20", "지정가FOK");
            //hogaGb[9] = new HogaGb("23", "시장가FOK");
            //hogaGb[10] = new HogaGb("26", "최유리FOK");
            //hogaGb[11] = new HogaGb("61", "시간외단일가매매");
            //hogaGb[12] = new HogaGb("81", "시간외종가");

            // 마켓코드 설정
            //marketCode[0] = new MarketCode("0", "장내");
            //marketCode[1] = new MarketCode("3", "ELW");
            //marketCode[2] = new MarketCode("4", "뮤추얼펀드");
            //marketCode[3] = new MarketCode("5", "신주인수권");
            //marketCode[4] = new MarketCode("6", "리츠");
            //marketCode[5] = new MarketCode("8", "ETF");
            //marketCode[6] = new MarketCode("9", "하이일드펀드");
            //marketCode[7] = new MarketCode("10", "코스닥");
            //marketCode[8] = new MarketCode("30", "제3시장");
        }
    }


    class Error
    {
        private static string errorMessage;

        Error()
        {
            errorMessage = "";
        }

        ~Error()
        {
            errorMessage = "";
        }

        public static string GetErrorMessage()
        {
            return errorMessage;
        }

        public static bool IsError(int nErrorCode)
        {
            bool bRet = false;

            switch (nErrorCode)
            {
                case KOAErrorCode.OP_ERR_NONE:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "정상처리";
                    bRet = true;
                    break;
                case KOAErrorCode.OP_ERR_LOGIN:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "사용자정보교환에 실패하였습니다. 잠시 후 다시 시작하여 주십시오.";
                    break;
                case KOAErrorCode.OP_ERR_CONNECT:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "서버 접속 실패";
                    break;
                case KOAErrorCode.OP_ERR_VERSION:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "버전처리가 실패하였습니다";
                    break;
                case KOAErrorCode.OP_ERR_SISE_OVERFLOW:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "시세조회 과부하";
                    break;
                case KOAErrorCode.OP_ERR_RQ_STRUCT_FAIL:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "REQUEST_INPUT_st Failed";
                    break;
                case KOAErrorCode.OP_ERR_RQ_STRING_FAIL:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "요청 전문 작성 실패";
                    break;
                case KOAErrorCode.OP_ERR_ORD_WRONG_INPUT:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "입력값 오류";
                    break;
                case KOAErrorCode.OP_ERR_ORD_WRONG_ACCNO:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "계좌비밀번호를 입력하십시오.";
                    break;
                case KOAErrorCode.OP_ERR_OTHER_ACC_USE:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "타인계좌는 사용할 수 없습니다.";
                    break;
                case KOAErrorCode.OP_ERR_MIS_2BILL_EXC:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "주문가격이 20억원을 초과합니다.";
                    break;
                case KOAErrorCode.OP_ERR_MIS_5BILL_EXC:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "주문가격은 50억원을 초과할 수 없습니다.";
                    break;
                case KOAErrorCode.OP_ERR_MIS_1PER_EXC:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "주문수량이 총발행주수의 1%를 초과합니다.";
                    break;
                case KOAErrorCode.OP_ERR_MID_3PER_EXC:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "주문수량은 총발행주수의 3%를 초과할 수 없습니다";
                    break;
                default:
                    errorMessage = "[" + nErrorCode.ToString() + "] :" + "알려지지 않은 오류입니다.";
                    break;
            }

            return bRet;
        }
    }
}

