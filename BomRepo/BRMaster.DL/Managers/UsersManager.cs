using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Collections.Generic;

namespace BomRepo.BRMaster.DL
{
    public class UsersManager : EntityManagers
    { 
        private const string useragentAutocad = "autocad";
        private const string useragentInventor = "inventor";
        private const string useragentWebApi = "web.api";
        private const string sysadminuser = "sysadmin";
        public UsersManager(BRMasterModel db) : base(db) {
            this.db = db;
        }
        public List<User> GetAll() {
            return db.Users.Where(u => u.Username.ToLower() != sysadminuser).ToList();
        }
        public User Get(string username, string password) {
            var user = db.Users.Where(u => u.Username == username & u.Password == password).FirstOrDefault();
            return user;
        }
        public User Get(string username) {
            var user = db.Users.Where(u => u.Username == username & u.Username.ToLower() != sysadminuser).FirstOrDefault();
            return user;
        }
        public User GetByToken(string token) {
            var user = db.Users.Where(u => u.AutocadToken == token | u.InventorToken == token | u.WebToken == token).FirstOrDefault();
            return user;
        }
        public bool SetToken(string username, string useragent, string token) {
            var user = db.Users.Where(u => u.Username == username).FirstOrDefault();
            if (user == null) return false;

            if (useragent == useragentAutocad) user.AutocadToken = token;
            if (useragent == useragentInventor) user.InventorToken = token;
            if (useragent == useragentWebApi) user.WebToken = token;
            db.SaveChanges();

            return true;
        }

        public User Add(User user) {
            var entities = db.Users.Where(u => u.Username.ToLower() == user.Username.ToLower());
            if (entities.Count() != 0) return null;

            User entity = new User() {
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
                Username = user.Username,
                Password = user.Password,
                Firstname = user.Firstname,
                Lastname = user.Lastname
            };
            db.Users.Add(entity);
            db.SaveChanges();

            return entity;
        }
        public bool Update(User user) {
            var entity = db.Users.Where(u => u.Username == user.Username & u.Username.ToLower() != sysadminuser).FirstOrDefault();
            if (entity == null) return false;

            entity.Firstname = user.Firstname;
            entity.Lastname = user.Lastname;
            db.SaveChanges();

            return true;
        }

        public bool Delete(string username) {
            var entity = db.Users.Where(u => u.Username == username & u.Username.ToLower() != sysadminuser).FirstOrDefault();
            if (entity == null) return false;

            entity.IsActive = false;
            db.SaveChanges();

            return true;
        }
    }
}
