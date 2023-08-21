using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Mandara.Business.Bus
{
    public abstract class BaseProfile : Profile
    {
        [Obsolete("Use AddMemberWiseTwoWayMap")]
        public void AddTwoWayMap<T, S>()
        {
            CreateMap<T, S>(MemberList.Destination);
            CreateMap<S, T>(MemberList.Source);
        }
        
        /// <summary>
        /// Create a map from T to S where S has less field than T
        /// which is typical for DTO object in IRM. Use the memberlist of
        /// S to map from T
        /// </summary>
        /// <typeparam name="T">Source object</typeparam>
        /// <typeparam name="S">Map object</typeparam>
        public void AddMemberWiseTwoWayMap<T, S>()
        {
            CreateMap<T, S>(MemberList.Destination);
            CreateMap<S, T>(MemberList.Source);
        }
        
        public void AddBasicTwoWayMap<T, S, TMember>(Expression<Func<T, TMember>> ignoreSourceMember)
        {
            CreateMap<T, S>();
            CreateMap<S, T>().ForMember(ignoreSourceMember, option => option.Ignore());
        }
        
        public void AddBasicTwoWayMap<T, S>()
        {
            CreateMap<T, S>();
            CreateMap<S, T>();
        }

        /// <summary>
        /// Create a map from T to S where S has less field than T
        /// which is typical for DTO object in IRM. Use the memberlist of
        /// S to map from T. Instruct Automapper to PreserveReferences to
        /// remove circular objects to been mapped multiple times in the target class
        /// </summary>
        /// <typeparam name="T">Source object</typeparam>
        /// <typeparam name="S">Map object</typeparam>
        public void AddTwoWayMapBlockingCircularRefs<T, S>()
        {
            CreateMap<T, S>(MemberList.Destination).PreserveReferences();
            CreateMap<S, T>(MemberList.Source).PreserveReferences();
        }

    }
}
