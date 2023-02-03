﻿using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System;
using Web_Api.online.Services;
using xNet;

namespace Web_Api.online.Models
{
    public class QiwiObject
    {
        CookieDictionary MyCokies;

        string UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36 OPR/43.0.2442.991";

        HttpRequest def = new HttpRequest();

        private QiwiService _qiwiService;

        public QiwiObject(QiwiService qiwiService)
        {
            _qiwiService = qiwiService;

            MyCokies = new CookieDictionary();
            def.Cookies = MyCokies;
            def.AllowAutoRedirect = !true;
            def.UserAgent = UA;

            def.IgnoreProtocolErrors = true;
        }

        public QiwiObject()
        {
            MyCokies = new CookieDictionary();
            def.Cookies = MyCokies;
            def.AllowAutoRedirect = !true;
            def.UserAgent = UA;

            def.IgnoreProtocolErrors = true;
        }

        public bool auth(string login, string pass)
        {
            bool check = false;
            using (var request = def)
            {
                HttpResponse resp = null;
                string tresp = string.Empty;
                request.Get("https://qiwi.com/");

                var pdata = ("{'login':'+" + login + "','password':'" + pass + "','captcha':''}").Replace('\'', '"');
                request.Referer = "https://qiwi.com/";
                request.AddHeader("Accept", "application/vnd.qiwi.sso-v1+json");

                resp = request.Post("https://sso.qiwi.com/cas/tgts", pdata, "application/json");
                tresp = resp.ToString();

                if (tresp.Contains(@"error"":{""message"))
                    check = false;
                else if (tresp.Contains(@"""ticket"":""TGT"))
                {
                    pdata = Regex.Match(tresp, @"ticket.{3}([^""]+)").Groups[1].Value;

                    pdata = ("{'ticket':'" + pdata + "','service':'https://qiwi.com/j_spring_cas_security_check'}").Replace('\'', '"');
                    request.Referer = "https://sso.qiwi.com/app/proxy?v=1";
                    resp = request.Post("https://sso.qiwi.com/cas/sts", pdata, "application/json");
                    tresp = resp.ToString();
                    pdata = Regex.Match(tresp, @"ticket.{3}([^""]+)").Groups[1].Value;
                    resp = request.Get("https://qiwi.com/j_spring_cas_security_check?ticket=" + pdata);

                    check = true;
                }

                resp = request.Get("https://qiwi.com/cards.action");

                tresp = resp.ToString();

                var tresp1 = tresp.IndexOf("<div class=\"account_current_amount\">");

                var tresp2 = tresp.Substring(tresp1 + 40);

                string endofstring = "\n";

                var indexTresp2 = tresp2.IndexOf(endofstring);

                var stringBalance = tresp2.Substring(0, indexTresp2);

                double balance = 0;

                if (Double.TryParse(stringBalance, out balance))
                {
                    //DataHelper.UpdateQiwiAccountBalance(login, balance);
                }
            }
            return check;
        }

        public void CheckQiwiAccountBalance(string login, string pass)
        {
            bool check = false;
            using (var request = def)
            {
                HttpResponse resp = null;
                string tresp = string.Empty;
                request.Get("https://qiwi.com/");

                var pdata = ("{'login':'+" + login + "','password':'" + pass + "','captcha':''}").Replace('\'', '"');
                request.Referer = "https://qiwi.com/";
                request.AddHeader("Accept", "application/vnd.qiwi.sso-v1+json");

                resp = request.Post("https://sso.qiwi.com/cas/tgts", pdata, "application/json");
                tresp = resp.ToString();

                if (tresp.Contains(@"error"":{""message"))
                    check = false;
                else if (tresp.Contains(@"""ticket"":""TGT"))
                {
                    pdata = Regex.Match(tresp, @"ticket.{3}([^""]+)").Groups[1].Value;

                    pdata = ("{'ticket':'" + pdata + "','service':'https://qiwi.com/j_spring_cas_security_check'}").Replace('\'', '"');
                    request.Referer = "https://sso.qiwi.com/app/proxy?v=1";
                    resp = request.Post("https://sso.qiwi.com/cas/sts", pdata, "application/json");
                    tresp = resp.ToString();
                    pdata = Regex.Match(tresp, @"ticket.{3}([^""]+)").Groups[1].Value;
                    resp = request.Get("https://qiwi.com/j_spring_cas_security_check?ticket=" + pdata);

                    check = true;
                }

                resp = request.Get("https://qiwi.com/settings.action");

                tresp = resp.ToString();

                var tresp1 = tresp.IndexOf("<div class=\"account_current_amount\">");

                var tresp2 = tresp.Substring(tresp1 + 40);

                string endofstring = "\n";

                var indexTresp2 = tresp2.IndexOf(endofstring);

                var stringBalance = tresp2.Substring(0, indexTresp2);

                double balance = 0;

                if (Double.TryParse(stringBalance, out balance))
                {
                    //DataHelper.UpdateQiwiAccountBalance(login, balance);
                }
            }
        }

        public bool payments(string account, double amount, string comment = "")
        {
            var data = ("{'id':'" + DateTimeToUnixTimestamp(DateTime.Now) +
                "','sum':{'amount':" + amount.ToString().Replace(',', '.') +
                ",'currency':'643'},'source':'account_643','paymentMethod':{'type':'Account','accountId':'643'},'comment':'" +
                comment
                + "','fields':{'account':'" + account + "','_meta_pay_partner':'','browser_user_agent_crc':'d7dc4c39'}}").Replace('\'', '"');

            using (var req = def)
            {
                req.Referer = "https://qiwi.com/payment/form.action?provider=99&state=confirm";

                req.AddHeader("Accept", "application/vnd.qiwi.v2+json");
                var resp = req.Post("https://qiwi.com/user/sinap/api/terms/99/payments/proxy.action", data, "application/json").ToString();

                dynamic jsondata = JsonConvert.DeserializeObject<dynamic>(resp);

                try
                {
                    if (jsondata.data.body.transaction.state.code == "Accepted")
                    {
                        return true;
                    }
                }
                catch { }

                return false;
            }
        }

        public bool paymentBankCard(string account, double amount)
        {
            var data = ("{'id':'" + DateTimeToUnixTimestamp(DateTime.Now) +
                "','sum':{'amount':" + amount.ToString().Replace(',', '.') +
                ",'currency':'643'},'source':'account_643','paymentMethod':{'type':'Account','accountId':'643'},'comment':'','fields':{'account':'" + account + "','_meta_pay_partner':'','browser_user_agent_crc':'d7dc4c39'}}").Replace('\'', '"');

            using (var req = def)
            {
                req.Referer = "https://qiwi.com/payment/form.action?provider=21013&state=confirm";

                req.AddHeader("Accept", "application/vnd.qiwi.v2+json");
                var resp = req.Post("https://qiwi.com/user/sinap/api/terms/21013/payments/proxy.action", data, "application/json").ToString();

                dynamic jsondata = JsonConvert.DeserializeObject<dynamic>(resp);

                try
                {
                    if (jsondata.data.body.transaction.state.code == "Accepted")
                    {
                        return true;
                    }
                }
                catch { }

                return false;
            }
        }

        static string DateTimeToUnixTimestamp(DateTime dateTime)
        {
            TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            return (span.TotalMilliseconds).ToString("0");
        }
    }

}
