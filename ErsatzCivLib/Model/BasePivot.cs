using System;

namespace ErsatzCivLib.Model
{
    /// <summary>
    /// Abstract base for each non-persistent "pivot" class.
    /// </summary>
    [Serializable]
    public abstract class BasePivot
    {
        /// <summary>
        /// The <see cref="Engine"/> which owns the instance.
        /// </summary>
        protected Engine Owner { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner"><see cref="Owner"/></param>
        protected BasePivot(Engine owner)
        {
            Owner = owner;
        }
    }
}
