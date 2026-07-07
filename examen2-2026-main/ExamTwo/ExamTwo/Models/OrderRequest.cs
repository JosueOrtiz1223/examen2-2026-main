using System;
using System.Collections.Generic;

namespace ExamTwo.Models;
    
    public class OrderRequest
    {
        public Dictionary<string, int> Order { get; set; }
        public Payment Payment { get; set; }
    }

    public class Payment
    {
        public int TotalAmount { get; set; }
        public List<int> Coins { get; set; }

    }