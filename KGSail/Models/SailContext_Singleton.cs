using Microsoft.EntityFrameworkCore;
using KGSail.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KGSail.Models
{
    /// <summary>
    /// Create a single reusable instance of the MvcMusicStoreContext class
    /// ... for use by metadata, partial and xUnit test classes.
    /// </summary>
    public class SailContext_Singleton
    {
        // single class-level instance, not accessed directly
        private static SailContext _context;
        // generic object to used to exclude others
        private static object syncLock = new object();

        /// <summary>
        /// Instantiate the context instance, if it doesn't yet exist
        /// </summary>
        /// <returns>MvcMusicStoreContext</returns>
        public static SailContext Context()
        {
            // Support multithreaded applications through 'double-checked locking':
            // - first program asking for the context locks everyone else out, then instantiates it
            // - when the lock is released, locked-out programs skip instantiation
            if (_context == null) // if instance already exists, skip to end
            {
                lock (syncLock) // first one here locks everyone else out until the instance is created
                {
                    if (_context == null) // people who were locked out now see instance & skip to end
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<SailContext>();
                        optionsBuilder.UseSqlServer(
                            @"Server=.\sqlexpress;Database=Sail;Trusted_Connection=True;");
                        _context = new SailContext(optionsBuilder.Options);
                    }
                }
            }
            return _context;
        }
    }
}
