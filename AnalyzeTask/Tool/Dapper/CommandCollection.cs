using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeTask.Tool.Dapper
{
    /// <summary>
    /// 命令集合
    /// </summary>
    public class CommandCollection
    {
        private readonly List<Command> _commands = null;
        public CommandCollection(params Command[] commands)
        {
            _commands = new List<Command>();
            if (commands != null)
            {
                _commands.AddRange(commands);
            }
        }
        public List<Command> GetCommands
        {
            get { return _commands; }
        }
        /// <summary>
        /// 向现有的命令集合加入一个命令
        /// </summary>
        /// <param name="command"></param>
        public void Add(Command command)
        {
            _commands.Add(command);
        }
        /// <summary>
        /// 向现有的命令集合删除一个命令
        /// </summary>
        /// <param name="command"></param>
        public void Remove(Command command)
        {
            _commands.Remove(command);
        }
        /// <summary>
        /// 批量加入命令
        /// </summary>
        /// <param name="commands"></param>
        public void BatchAdd(List<Command> commands)
        {
            _commands.AddRange(commands);
        }

        /// <summary>
        /// 批量加入命令
        /// </summary>
        /// <param name="commands"></param>
        public void BatchAdd(Command[] commands)
        {
            _commands.AddRange(commands);
        }
        public void Clear()
        {
            _commands.Clear();
        }
        public bool Any(Func<Command, bool> predicate = null)
        {
            if (predicate == null)
            {
                return _commands.Any();
            }
            else
            {
                return _commands.Any(predicate);
            }

        }
        
        public IEnumerable<IGrouping<string, Command>> GetDbType()
        {
            return _commands.GroupBy(d => d.DbType);
        }
    }
}
