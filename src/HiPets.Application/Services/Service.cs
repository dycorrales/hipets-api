using FluentValidation.Results;
using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Helpers.Utils;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HiPets.Application.Services
{
    public abstract class Service<T> where T : Entity
    {
        private readonly IMediatorHandler _mediator;
        private readonly IRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Service(IMediatorHandler mediator, IUnitOfWork unitOfWork, IRepository<T> repository)
        {
            _mediator = mediator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public virtual void Insert(T entity)
        {
            _repository.Insert(entity);
            var response = _unitOfWork.Commit();

            if (!response.Success)
            {
                RaiseNotification(nameof(entity), "Erro ao salvar os dados", NotificationType.Error);
                return;
            }

            RaiseNotification(nameof(entity), "Dados salvados com sucesso", NotificationType.Info);
        }

        public virtual void InsertRange(IEnumerable<T> entities)
        {
            _repository.InsertRange(entities);
            var response = _unitOfWork.Commit();

            if (!response.Success)
            {
                RaiseNotification(nameof(T), "Erro ao salvar os dados", NotificationType.Error);
                return;
            }

            RaiseNotification(nameof(T), "Dados salvados com sucesso", NotificationType.Info);
        }

        public virtual void Update(T entity)
        {
            _repository.Update(entity);
            var response = _unitOfWork.Commit();

            if (!response.Success)
            {
                RaiseNotification(nameof(entity), "Erro ao salvar os dados", NotificationType.Error);
                return;
            }

            RaiseNotification(nameof(entity), "Dados salvados com sucesso", NotificationType.Info);
        }

        public virtual void Delete(T entity)
        {
            if(entity == null)
            {
                RaiseNotification(nameof(entity), "Elemento não existe", NotificationType.Error);
                return;
            }

            entity.Delete();

            _repository.Update(entity);
            var response = _unitOfWork.Commit();

            if (!response.Success)
            {
                RaiseNotification(nameof(entity), "Erro ao excluir o elemento", NotificationType.Error);
                return;
            }

            RaiseNotification(nameof(entity), "Elemento excluido com sucesso", NotificationType.Info);
        }

        public bool ExistsElements()
        {
            return _repository.Any(entity => entity.Status == Status.Active);
        }

        public virtual IEnumerable<T> FindAll()
        {
            return _repository.FindAll();
        }

        public virtual T FindById(Guid id)
        {
            return _repository.FindById(id);
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return _repository.Any(predicate);
        }

        public void RaiseNotification(string key, string value, NotificationType type)
        {
            _mediator.RaiseNotification(new DomainNotification(key, value, type));
        }
        
        public void RaiseNotification(ValidationResult validationResult, NotificationType? type = null)
        {
            foreach (var error in validationResult.Errors)
            {
                RaiseNotification(error.PropertyName, error.ErrorMessage, type ?? NotificationType.Error);
            }
        }
    }
}
