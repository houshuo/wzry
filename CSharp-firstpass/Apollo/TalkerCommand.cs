namespace Apollo
{
    using ApolloTdr;
    using System;
    using System.Runtime.CompilerServices;

    internal class TalkerCommand
    {
        public TalkerCommand(CommandDomain domain, CommandValueType type)
        {
            this.Domain = domain;
            this.Command = new CommandValue(type);
        }

        public TalkerCommand(CommandDomain domain, IPackable obj)
        {
            if (obj == null)
            {
                throw new Exception("TalkerCommand Invalid Argument");
            }
            this.Domain = domain;
            this.Command = new CommandValue(obj.GetType().ToString());
        }

        public TalkerCommand(CommandDomain domain, string command)
        {
            if (command == null)
            {
                throw new Exception("TalkerCommand(string) Invalid Argument");
            }
            this.Domain = domain;
            this.Command = new CommandValue(command);
        }

        public TalkerCommand(CommandDomain domain, uint command)
        {
            this.Domain = domain;
            this.Command = new CommandValue(command);
        }

        public override bool Equals(object obj)
        {
            TalkerCommand command = obj as TalkerCommand;
            if ((command == null) || (command.Command == null))
            {
                return false;
            }
            if (this.Domain != command.Domain)
            {
                return false;
            }
            return this.Command.Equals(command.Command);
        }

        public override int GetHashCode()
        {
            return (this.Command.GetHashCode() + this.Domain.GetHashCode());
        }

        public override string ToString()
        {
            return string.Format("Domain:{0}, Value:{1}", this.Domain, this.Command);
        }

        public CommandValue Command { get; set; }

        public CommandDomain Domain { get; set; }

        public enum CommandDomain
        {
            App = 1,
            TSS = 0xff
        }

        public class CommandValue
        {
            public CommandValue(TalkerCommand.CommandValueType type)
            {
                this.Type = type;
            }

            public CommandValue(string value)
            {
                this.Type = TalkerCommand.CommandValueType.String;
                this.StringValue = value;
            }

            public CommandValue(uint value)
            {
                this.Type = TalkerCommand.CommandValueType.Integer;
                this.IntegerValue = value;
            }

            public override bool Equals(object obj)
            {
                TalkerCommand.CommandValue value2 = obj as TalkerCommand.CommandValue;
                if (value2 == null)
                {
                    return false;
                }
                if (this.Type != value2.Type)
                {
                    return false;
                }
                if (this.Type == TalkerCommand.CommandValueType.Integer)
                {
                    return (this.IntegerValue == value2.IntegerValue);
                }
                return ((this.Type == TalkerCommand.CommandValueType.Raw) || (this.StringValue == value2.StringValue));
            }

            public override int GetHashCode()
            {
                int hashCode = this.Type.GetHashCode();
                if (this.Type == TalkerCommand.CommandValueType.Integer)
                {
                    return (hashCode + this.IntegerValue.GetHashCode());
                }
                if (this.Type == TalkerCommand.CommandValueType.Raw)
                {
                    return (hashCode + this.Type.GetHashCode());
                }
                return (hashCode + this.StringValue.GetHashCode());
            }

            public override string ToString()
            {
                return string.Format("Type:{0}, StringValue:{1}, IntegerValue:{2}", this.Type, this.StringValue, this.IntegerValue);
            }

            public uint IntegerValue { get; set; }

            public string StringValue { get; set; }

            public TalkerCommand.CommandValueType Type { get; private set; }
        }

        public enum CommandValueType
        {
            Raw,
            String,
            Integer
        }
    }
}

