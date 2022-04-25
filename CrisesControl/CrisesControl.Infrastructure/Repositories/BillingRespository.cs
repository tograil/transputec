﻿using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories {
    public class BillingRespository : IBillingRepository {
        private readonly CrisesControlContext _context;

        public BillingRespository(CrisesControlContext context) {
            _context = context;
        }

        public async Task<BillingPaymentProfile> GetPaymentProfile(int companyID) {
            BillingPaymentProfile BillInfo = new BillingPaymentProfile();

            try {

                var pCompanyId = new SqlParameter("@CompanyID", companyID);
                var Profile = _context.Set<CompanyPaymentProfile>().FromSqlRaw("exec Pro_Company_GetCompanyAccount_PaymentProfile {0}", pCompanyId).ToList().FirstOrDefault();

                if (Profile != null) {
                    BillInfo.Profile = Profile;

                    List<string> stopped_comms = new List<string>();

                    var subscribed_method = _context.Set<CompanySubscribedMethod>().FromSqlRaw("exec Pro_Get_Company_Subscribed_Methods {0}", pCompanyId).ToListAsync()
                        .Result.Select(c => c.MethodCode).ToList();

                    if (subscribed_method.Contains("EMAIL")) {
                        if (Profile.MinimumEmailRate > 0) {
                            stopped_comms.Add("Email");
                        }
                    }
                    if (subscribed_method.Contains("PHONE")) {
                        if (Profile.MinimumPhoneRate > 0) {
                            stopped_comms.Add("Phone");
                        }
                    }
                    if (subscribed_method.Contains("PHONE")) {
                        if (Profile.MinimumPhoneRate > 0) {
                            stopped_comms.Add("Conference");
                        }
                    }
                    if (subscribed_method.Contains("TEXT")) {
                        if (Profile.MinimumTextRate > 0) {
                            stopped_comms.Add("Text");
                        }
                    }
                    if (subscribed_method.Contains("PUSH")) {
                        if (Profile.MinimumPushRate > 0) {
                            stopped_comms.Add("Push");
                        }
                    }
                    if (stopped_comms.Count > 0) {
                        string stopped_service = string.Join(", ", stopped_comms);
                        BillInfo.PaidServices = stopped_service;
                    }
                }
            } catch (Exception ex) {

            }
            return BillInfo;
        }
    }
}