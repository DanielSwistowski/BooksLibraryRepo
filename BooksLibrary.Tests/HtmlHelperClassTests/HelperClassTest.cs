using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using BooksLibrary.HtmlHelperClass;
using BooksLibrary.Business.Models;

namespace BooksLibrary.Tests.HtmlHelperClassTests
{
    [TestClass]
    public class HelperClassTest
    {
        [TestMethod]
        public void can_change_bool_value_to_yes_or_no()
        {
            HtmlHelper helper = null;
            bool testValue = false;

            var result = HelperClass.BoolToYesOrNo(helper, testValue);

            Assert.IsTrue(result.ToString() == "Nie");
        }

        [TestMethod]
        public void can_translate_enum_gender_to_string()
        {
            HtmlHelper helper = null;
            var gender = Gender.Male;

            var result = HelperClass.GenderTranstateToPL(helper, gender);

            Assert.IsTrue(result.ToString() == "Mężczyzna");
        }

        [TestMethod]
        public void can_change_integer_to_string()
        {
            HtmlHelper helper = null;
            int quantity = 20;

            var result = HelperClass.QuantityToYesOrNo(helper, quantity);

            Assert.IsTrue(result.ToString() == "Tak");
        }
    }
}
