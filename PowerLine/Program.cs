using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerLine
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }

        // Опишите класс автомобиль у которого есть базовые параметры в виде типа ТС, среднего расхода
        // топлива, объем топливного бака, скорость. 
        // Реализуйте на его основе классы легковой автомобиль, грузовой автомобиль, спортивный автомобиль.
        public abstract class Vehicle
        {
            public Vehicle(string type, double fuelConsumption, double fuelCapacity, double maxSpeed)
                : base()
            {
                Type = type;
                FuelConsumption = fuelConsumption;
                FuelCapacity = fuelCapacity;
                MaxSpeed = maxSpeed;
            }

            /// <summary>
            /// Тип транспортного средства
            /// </summary>
            public string Type { get; }

            /// <summary>
            /// Расход топлива без груза, л/км.
            /// </summary>
            public double FuelConsumption { get; }

            /// <summary>
            /// Объем топливного бака, л.
            /// </summary>
            public double FuelCapacity { get; }

            /// <summary>
            /// Максимальная скрорость автомобиля, км/ч
            /// </summary>
            public double MaxSpeed { get; }

            /// <summary>
            /// Коэфициент уменьшение запаса хода в зависимости от загрузки автомобиля.
            /// Для каждой разновидности автомобиля свой.
            /// </summary>
            protected abstract double CargoKoef { get; }

            /// <summary>
            /// Расход топлива с грузом, л/км.
            /// </summary>
            public double CargoFuelConsumption => FuelConsumption / (1 - CargoKoef);

            /// <summary>
            /// Текущий объем топлива, л.
            /// </summary>
            public double FuelVolume { get; set; }

            // Опишите метод, с помощью которого можно вычислить сколько автомобиль может проехать
            // на полном баке топлива.
            /// <summary>
            /// Запас хода при полном баке топлива, км.
            /// </summary>
            /// <returns></returns>
            public double GetMaxFuelDistance()
            {
                double result = FuelCapacity / CargoFuelConsumption;
                if (result < 0)
                    result = 0;
                return result;
            }

            // Метод для отображения текущей информации о состоянии запаса хода в зависимости от
            // пассажиров и груза. 
            // Опишите метод, с помощью которого можно вычислить сколько автомобиль может проехать
            // на остаточном количестве топлива в баке на данный момент.
            /// <summary>
            /// Запас хода при текущем объеме топлива, км.
            /// </summary>
            /// <returns></returns>
            public double GetFuelDistance()
            {
                return GetFuelDistance(FuelVolume);
            }

            /// <summary>
            /// Запас хода при заданном объеме топлива, км. 
            /// </summary>
            /// <returns></returns>
            public double GetFuelDistance(double fuelVolume)
            {
                double result = fuelVolume / CargoFuelConsumption;
                if (result < 0)
                    result = 0;
                return result;
            }

            // Метод, который на основе параметров количества топлива и
            // заданного расстояния вычисляет за сколько автомобиль его преодолеет.
            /// <summary>
            /// Время на поездку.
            /// Непонятное условие "количества топлива и заданного расстояния".
            /// </summary>
            /// <param name="fuelVolume"></param>
            /// <param name="distance"></param>
            /// <returns></returns>
            public double GetTripTime(double fuelVolume, double distance)
            {
                distance = Math.Min(GetFuelDistance(fuelVolume), distance);
                double result = distance / MaxSpeed;
                return result;
            }

            /// <summary>
            /// Получить список всех проблем.
            /// </summary>
            /// <param name="errors"></param>
            protected virtual void Diagnose(List<string> errors)
            {
                if (FuelVolume == 0)
                    errors.Add("Автомобиль не заправлен.");
                if (CargoKoef >= 1)
                    errors.Add("Загруженность автомобиля превышает технологические нормы.");
            }

            /// <summary>
            /// Проверка готовности к поездке.
            /// </summary>
            /// <param name="reason">Список проблем</param>
            /// <returns>Готов ли автомобиль к поездке.</returns>
            public virtual bool CheckTripReady(out string[] reason)
            {
                List<string> errors = new List<string>();
                Diagnose(errors);
                reason = errors.Any()
                    ? errors.ToArray()
                    : null;
                return !errors.Any();
            }
        }

        public class Car : Vehicle
        {
            public Car(string type, double fuelConsumption, double fuelCapacity, double maxSpeed,
                uint passengerCapacity)
                : base(type, fuelConsumption, fuelCapacity, maxSpeed)
            {
                PassengerCapacity = passengerCapacity;
            }

            // У легкового автомобиля добавьте параметр количество перевозимых пассажиров.
            // На основе данного параметра может изменяться запас хода.
            // Предусмотрите проверку на допустимое количество пассажиров.
            /// <summary>
            /// Допустимое количество пассажиров.
            /// </summary>
            public uint PassengerCapacity { get; }

            /// <summary>
            /// Текущее количество пассажиров.
            /// </summary>
            public uint PassengerCount { get; set; }

            // Каждый дополнительный пассажир уменьшает запас хода на дополнительные 6%. 
            protected override double CargoKoef => PassengerCount * 0.06;

            protected override void Diagnose(List<string> errors)
            {
                base.Diagnose(errors);
                if (PassengerCount > PassengerCapacity)
                    errors.Add($"Автомобиль перегружен. Количество пассажиров в салоне ({PassengerCount} чел) "
                        + $"превышает норму ({PassengerCapacity} чел).");
            }
        }

        public class SportCar : Car
        {
            public SportCar(string type, double fuelConsumption, double fuelCapacity, double maxSpeed,
                uint passengerCapacity)
                : base(type, fuelConsumption, fuelCapacity, maxSpeed, passengerCapacity)
            { }
        }

        public class Truck : Vehicle
        {
            public Truck(string type, double fuelConsumption, double fuelCapacity, double maxSpeed,
                double cargoCapacity)
                : base(type, fuelConsumption, fuelCapacity, maxSpeed)
            {
                CargoCapacity = cargoCapacity;
            }

            // Класс грузового автомобиля дополните параметром грузоподъемность.
            // Также, как и у легкового автомобиля, грузоподъемность влияет на запас хода автомобиля.
            // Дополните класс проверкой может ли автомобиль принять полный груз на борт.
            /// <summary>
            /// Грузоподъемность, т.
            /// </summary>
            public double CargoCapacity { get; }

            /// <summary>
            /// Вес груза, т.
            /// </summary>
            public double CargoVolume { get; }

            // Каждые дополнительные 200кг веса уменьшают запас хода на 4%.
            protected override double CargoKoef => (CargoVolume / 5) * 0.04;

            protected override void Diagnose(List<string> errors)
            {
                base.Diagnose(errors);
                if (CargoVolume > CargoCapacity)
                    errors.Add($"Автомобиль перегружен. Вес груза ({CargoVolume} т) "
                        + $"превышает норму ({CargoCapacity} т).");
            }
        }

    }

}
