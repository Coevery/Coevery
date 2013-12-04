using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Data;
using Coevery.Projections.Models;

namespace Coevery.Projections.Services {
    public interface IGroupService : IDependency {
        void MoveUp(int groupId);
        void MoveDown(int groupId);
    }

    public class GroupService : IGroupService {
        private readonly IRepository<LayoutGroupRecord> _repository;

        public GroupService(IRepository<LayoutGroupRecord> repository) {
            _repository = repository;
        }

        public void MoveUp(int groupId) {
            var group = _repository.Get(groupId);

            // look for the previous action in order in same rule
            var previous = _repository.Table
                .Where(x => x.Position < group.Position && x.LayoutRecord.Id == group.LayoutRecord.Id)
                .OrderByDescending(x => x.Position)
                .FirstOrDefault();

            // nothing to do if already at the top
            if (previous == null) {
                return;
            }

            // switch positions
            var temp = previous.Position;
            previous.Position = group.Position;
            group.Position = temp;
        }

        public void MoveDown(int groupId) {
            var group = _repository.Get(groupId);

            // look for the next action in order in same rule
            var next = _repository.Table
                .Where(x => x.Position > group.Position && x.LayoutRecord.Id == group.LayoutRecord.Id)
                .OrderBy(x => x.Position)
                .FirstOrDefault();

            // nothing to do if already at the end
            if (next == null) {
                return;
            }

            // switch positions
            var temp = next.Position;
            next.Position = group.Position;
            group.Position = temp;
        }
    }
}