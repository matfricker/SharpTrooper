using System;
using System.Collections.Generic;

namespace SharpTrooper.Entities
{
    public class SharpEntityResults<T> : SharpEntity where T : SharpEntity
    {
        public string Previous
        {
            get;
            set;
        }

        public string Next
        {
            get;
            set;
        }

        public string PreviousPageNo
        {
            get;
            set;
        }

        public string NextPageNo
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public IList<T> Results
        {
            get;
            set;
        }
    }
}