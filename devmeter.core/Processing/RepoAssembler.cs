using devmeter.core.Github.Models;
using devmeter.ui.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace devmeter.core.Processing
{
    public class RepoAssembler
    {
        private Repo _repo;

        public RepoAssembler(Repo repo)
        {
            _repo = repo;
        }

        public Repo GetRepo() => _repo;

        public void UpdateGeneralInfo(GitHubGeneralInformation info)
        {
            _repo.Name = info.Name;
        }

        public void UpdateCommits(string commits)
        {
            _repo.Commits = commits;
        }

        public void UpdateCommitsInLast30Days(int commits)
        {
            _repo.CommitsInLast30Days = String.Format($"{commits:n0}"); 
        }

        public void UpdateContributors(string contributors)
        {
            _repo.Contributors = contributors;
        }

    }
}
