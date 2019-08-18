using System;
using System.Collections.Generic;

namespace CQRS
{

    public class Person
    {
        private int age;
        EventBroker broker;
        public Person(EventBroker broker)
        {
            this.broker = broker;
            broker.Commands += Broker_Commands;
            broker.Queries += Broker_Queries;
        }

        private void Broker_Queries(object sender, Query query)
        {
            var ageQuery = query as AgeQuery;
            if (ageQuery != null && ageQuery.Target == this)
            {
                ageQuery.Result = this.age;
            }
        }

        private void Broker_Commands(object sender, Command command)
        {
            var changeAgeCommand = command as ChangeAgeCommand;
            if (changeAgeCommand != null && changeAgeCommand.Target == this)
            {
                this.age = changeAgeCommand.age;
            }
        }
    }

    public class EventBroker
    {
        //1. All events that happened
        public List<Event> AllEvents = new List<Event>();

        //2. Commands
        public event EventHandler<Command> Commands;
        //3. Queries
        public event EventHandler<Query> Queries;

        public void Command(Command c)
        {
            Commands?.Invoke(this, c);
        }

        public T Query<T>(Query q)
        {
            Queries?.Invoke(this, q);
            return (T)q.Result;
        }
    }

    public class Query
    {
        public object Result;
    }

    public class AgeQuery : Query
    {
        public Person Target;
    }

    public class Command : EventArgs
    {

    }

    public class ChangeAgeCommand : Command
    {
        public Person Target;
        public int age;

        public ChangeAgeCommand(Person target, int age)
        {
            Target = target;
            this.age = age;
        }
    }

    public class Event
    {

    }


    class Program
    {
        static void Main(string[] args)
        {
            var eventBroker = new EventBroker();
            var person = new Person(eventBroker);
            eventBroker.Command(new ChangeAgeCommand(person, 21));
            int age = eventBroker.Query<int>(new AgeQuery { Target = person });
            Console.WriteLine(age);
        }
    }
}
