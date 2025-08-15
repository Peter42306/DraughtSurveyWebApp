using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DraughtSurveyWebApp.Utils.Utils;

namespace DraughtSurveyWebApp.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void AreEqual_Doubles_Exactmatch_ReturnTrue()
        {            
            Assert.True(AreEqual(1.0, 1.0));
        }

        //[Theory]
        //[InlineData(1.0, 1.0, true)]
        //[InlineData(1.0, 1.00005, true)]  // в пределах tolerance 0.0001
        //[InlineData(1.0, 1.0002, false)]  // вне tolerance
        //public void AreEqual_Doubles_WhenBothHaveValue(double a, double b, bool expected)
        //{            
        //    Assert.Equal(expected, AreEqual(a, b));            
        //}

        //[Theory]
        //[InlineData(true, true, true)]
        //[InlineData(false, false, true)]
        //[InlineData(true, false, false)]
        //public void AreEqual_Bools_WhenBothHaveValue(bool a, bool b, bool expected)
        //{            
        //    Assert.Equal(expected, AreEqual(a, b));            
        //}

        //[Theory]
        //[InlineData(null, null, false)]
        //[InlineData(null, 1.0, false)]
        //[InlineData(1.0, null, false)]
        //public void AreEqual_Doubles_WithNulls(double? a, double? b, bool expected)
        //{            
        //    Assert.Equal(expected, AreEqual(a, b));            
        //}

        //[Theory]
        //[InlineData(null, null, false)]
        //[InlineData(null, true, false)]
        //[InlineData(false, null, false)]
        //public void AreEqual_Bools_WithNulls(bool? a, bool? b, bool expected)
        //{           
        //    Assert.Equal(expected, AreEqual(a, b));            
        //}
    }
}
