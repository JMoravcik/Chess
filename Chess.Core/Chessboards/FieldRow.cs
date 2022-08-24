using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public class FieldRow : IEnumerable<Field>
    {
        Field[] Fields = new Field[8];
        internal FieldRow(int row)
        {
            Row = row;
            for (int i = 0; i < Game.Cols; i++)
            {
                Fields[i] = new Field(Row, i);
            }
        }

        public int Row { get; }

        public Field this[int index]
        {
            get
            {
                if (index >= 0 && index < Game.Cols)
                {
                    return Fields[index];
                }
                return null;
            }
        }

        public IEnumerator<Field> GetEnumerator()
        {
            foreach (var field in Fields)
            {
                yield return field;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
