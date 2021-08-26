using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Calculator_SeleniumTests
{
    public class CalculatorTests
    {
        private readonly IWebDriver _driver;
        private const string CalculatorTemporaryOutputId = "calculator_state";
        private const string CalculatorResultId = "calculator_output";
        private const string PlusButtonId = "plusButton";
        private const string EqualButtonId = "equalButton";
        private const string MinusButtonId = "minusButton";
        private const string MultiplyButtonId = "multiplyButton";
        private const string DivideButtonId = "divideButton";
        private const string LeftBracketButtonId = "leftBracket";

        public delegate string Operation(double a, double b);

        private static readonly object[] ComplexTestCases =
        {
            new object[]
            {
                new[] { "num0", "num1", "num2" }, "12"
            },
            new object[]
            {
                new[] { "num3", "num4", "num5" }, "345"
            }
        };

        private static readonly object[] ComplexTestCasesWithResult =
        {
            new object[]
            {
                new[] { LeftBracketButtonId, "num4", MultiplyButtonId, LeftBracketButtonId, "num5" }, "Invalid brackets count!"
            }
        };

        private static readonly object[] OperationsTestCases =
        {
            new object[] { PlusButtonId, new Operation((a, b) => (a + b).ToString())},
            new object[] { MinusButtonId, new Operation((a, b) => (a - b).ToString()) },
            new object[] { MultiplyButtonId, new Operation( (a, b) => (a * b).ToString()) },
            new object[]
            {
                DivideButtonId, new Operation(
                    (a, b) =>
                    {
                        if (b != 0)
                        {
                            return  (a / b).ToString();
                        }
                        else if (a != 0)
                        {
                            return "Infinity";
                        }

                        return "NaN";
                    })
            }
        };

        public CalculatorTests()
        {
            _driver = new ChromeDriver(@"C:\Users\WorkUser\Desktop\WebDrivers");
            _driver.Url = @"C:\Users\WorkUser\Desktop\MyWeb—alculator\index.html";
        }

        [TestCase("num0", "0")]
        [TestCase("num1", "1")]
        [TestCase("num2", "2")]
        [TestCase("num3", "3")]
        [TestCase("num4", "4")]
        [TestCase("num5", "5")]
        [TestCase("num6", "6")]
        [TestCase("num7", "7")]
        [TestCase("num8", "8")]
        [TestCase("num9", "9")]
        public void Button_WhenClicked_ShouldDisplayOnTextBox(string buttonId, string expectedResult)
        {
            var button = _driver.FindElement(By.Id(buttonId));
            button.Click();

            var output = _driver.FindElement(By.Id(CalculatorTemporaryOutputId));
            output.GetAttribute("value").Should().Be(expectedResult);
        }

        [Test, TestCaseSource("OperationsTestCases")]
        public void Test_WhenAOperationBClicked_ShouldReturnOperationResult(string operationButtonId, Operation operation)
        {
            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    var operationButton = _driver.FindElement(By.Id(operationButtonId));
                    var equalButton = _driver.FindElement(By.ClassName(EqualButtonId));
                    var output = _driver.FindElement(By.Id(CalculatorResultId));

                    var buttonA = _driver.FindElement(By.Id($"num{i}"));
                    buttonA.Click();

                    operationButton.Click();

                    var buttonB = _driver.FindElement(By.Id($"num{j}"));
                    buttonB.Click();

                    equalButton.Click();

                    output.GetAttribute("value").Should().Be(operation.Invoke(i, j));

                    _driver.Navigate().Refresh();
                }
            }
        }

        [Test, TestCaseSource("ComplexTestCases")]
        public void Test_WhenMultiplyButtonsClicked_ShouldViewExpectedValue(string[] buttons, string expectedResult)
        {
            foreach (var buttonId in buttons)
            {
                var button = _driver.FindElement(By.Id(buttonId));
                button.Click();
            }

            var resultTextBox = _driver.FindElement(By.Id(CalculatorTemporaryOutputId));
            resultTextBox.GetAttribute("value").Should().Be(expectedResult);
        }

        [Test, TestCaseSource("ComplexTestCasesWithResult")]
        public void Test_WhenMultiplyButtonsClicked_ShouldCalculateExpectedValue(string[] buttons, string expectedResult)
        {
            foreach (var buttonId in buttons)
            {
                var button = _driver.FindElement(By.Id(buttonId));
                button.Click();
            }

            var calculateButton = _driver.FindElement(By.Id(EqualButtonId));
            calculateButton.Click();

            var resultTextBox = _driver.FindElement(By.Id(CalculatorResultId));
            resultTextBox.GetAttribute("value").Should().Be(expectedResult);
        }

        [TearDown]
        public void AfterTests()
        {
            _driver.Navigate().Refresh();
        }

        [OneTimeTearDown]
        public void AfterAllTests()
        {
            _driver.Quit();
        }
    }
}