using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Management;
using System.Reflection;
using WmiSharp.Test.Helpers;

namespace WmiSharp.Test.Tests
{
    [TestClass]
    public class PropertyConvert
    {

        [TestClass]
        public class DateTimeTests
        {
            /// <summary>
            /// Check if a WMI date property translates to a <see cref="DateTime"/>.
            /// </summary>
            /// <param name="value"></param>
            [TestMethod]
            [DataRow("20190628212755.500000-420")]
            public void DateTime_FormattedString_Test(string value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.DateTimeNonNullable), value);
                DateTime expected = new DateTime(636973540755000000);
                Assert.AreEqual(expected, actual.DateTimeNonNullable);
            }

            /// <summary>
            /// Check if a null WMI date property translates to <see cref="DateTime.MinValue"/>.
            /// </summary>
            /// <param name="value"></param>
            [TestMethod]
            [DataRow(null)]
            public void DateTime_NonNullableMinValue_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.DateTimeNonNullable), value);
                Assert.AreEqual(DateTime.MinValue, actual.DateTimeNonNullable);
            }

            /// <summary>
            /// Check if a null WMI date property translates to a <see cref="Nullable{DateTime}" />.
            /// </summary>
            /// <param name="value"></param>
            [TestMethod]
            [DataRow(null)]
            public void DateTime_Nullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.DateTimeNullable), value);
                Assert.AreEqual(null, actual.DateTimeNullable);
            }

            /// <summary>
            /// Check if an empty WMI date property translates to a <see cref="Nullable{DateTime}" />.
            /// </summary>
            /// <param name="value"></param>
            [TestMethod]
            [DataRow("")]
            public void DateTime_EmptyStringNonNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.DateTimeNonNullable), value);
                Assert.AreEqual(DateTime.MinValue, actual.DateTimeNonNullable);
            }

            /// <summary>
            /// Check if an empty WMI date property translates to a <see cref="Nullable{DateTime}" />.
            /// </summary>
            /// <param name="value"></param>
            [TestMethod]
            [DataRow("")]
            public void DateTime_EmptyStringNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.DateTimeNullable), value);
                Assert.AreEqual(null, actual.DateTimeNullable);
            }
        }


        [TestClass]
        public class EnumTests
        {
            [TestMethod]
            [DataRow(ExampleEnum.Test1)]
            public void Enum_NonNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNonNullable), value);
                Assert.AreEqual(ExampleEnum.Test1, actual.EnumTestNonNullable);
            }

            [TestMethod]
            [DataRow("")]
            public void Enum_EmptyStringNonNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNonNullable), value);
                Assert.AreEqual(default(ExampleEnum), actual.EnumTestNonNullable);
            }

            [TestMethod]
            [DataRow("")]
            public void Enum_EmptyStringNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNullable), value);
                Assert.AreEqual(null, actual.EnumTestNullable);
            }

            [TestMethod]
            [DataRow(999)]
            public void Enum_IntOutOfRangeNonNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNonNullable), value);
                Assert.AreEqual(default(ExampleEnum), actual.EnumTestNonNullable);
            }

            [TestMethod]
            [DataRow(3)]
            public void Enum_IntNonNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNonNullable), value);
                Assert.AreEqual(ExampleEnum.Test3, actual.EnumTestNonNullable);
            }

            [TestMethod]
            [DataRow("2")]
            public void Enum_IntStringNonNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNonNullable), value);
                Assert.AreEqual(ExampleEnum.Test2, actual.EnumTestNonNullable);
            }

            [TestMethod]
            [DataRow(null)]
            public void Enum_Nullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNonNullable), value);
                Assert.AreEqual(null, actual.EnumTestNullable);
            }

            [TestMethod]
            [DataRow(999)]
            public void Enum_IntOutOfRangeNullable_Test(object value)
            {
                var actual = PropertyHelper.SetPropertyValue<ConversionTest>(nameof(ConversionTest.EnumTestNonNullable), value);
                Assert.AreEqual(null, actual.EnumTestNullable);
            }
        }

    }




    public class ConversionTest
    {
        public DateTime DateTimeNonNullable { get; set; }
        public DateTime? DateTimeNullable { get; set; }

        public ExampleEnum EnumTestNonNullable { get; set; }

        public ExampleEnum? EnumTestNullable { get; set; }
    }



}
