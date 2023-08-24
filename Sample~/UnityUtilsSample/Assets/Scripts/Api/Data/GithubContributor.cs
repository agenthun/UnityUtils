using System;

namespace Api.Data
{
    [Serializable]
    public class GithubContributor
    {
        public string login;
        public int contributions;
    }
}