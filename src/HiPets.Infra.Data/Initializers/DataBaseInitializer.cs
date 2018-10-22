
using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Infra.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiPets.Infra.Data.Initializers
{
    public sealed class DataBaseInitializer
    {
        private readonly Context _context;

        public DataBaseInitializer(Context context)
        {
            _context = context;

        }

        public void Initialize(Guid userAdminId, Guid userAdopterId)
        {
            _context.Database.Migrate();
            _context.Database.EnsureCreated();
            InitializeObjects(userAdminId, userAdopterId);
        }

        public void InitializeObjects(Guid userAdminId, Guid userAdopterId)
        {
            AddAnimals();
            AddAdopters(userAdminId, userAdopterId);
            AddAdoptions(userAdopterId);
        }

        private void AddAnimals()
        {
            CreateAnimal(new Guid("5ef4c133-1cdd-4ef4-8726-22b285106a99"), "Toto", "Vira lata", 3, Color.Brown, "É um cachorro tranquilo, amistoso e brincalhão", AnimalType.Dog, "http://tudosobrecachorros.com.br/wp-content/uploads/10-motivos-pra-adotar-um-vira-lata.jpg");
            CreateAnimal(new Guid("b84e260f-98ea-4c4a-9274-11c4fea3061f"), "Pluto", "Beagle", 1, Color.White, "É um cachorro bem ativo, um pouco bravo, mais é muito fiel e amigo das crianças", AnimalType.Dog, "http://tudosobrecachorros.com.br/wp-content/uploads/Beagle-03.jpg");
            CreateAnimal(new Guid("62236f3e-e474-47ce-b960-4c719ebf9fb2"), "Laila", "Poodle", 7, Color.Gray, "Gosta de brincar na grama e é bem tranquila", AnimalType.Dog, "https://media.mnn.com/assets/images/2016/04/brown-standard-poodle.jpg.638x0_q80_crop-smart.jpg");
            CreateAnimal(new Guid("cae8da45-db5b-4fc5-ad92-7bca814a8d5f"), "Bolinha", "Persa", 2, Color.Yellow, "Gosta de comer muito, é tranquilo e carinhoso", AnimalType.Cat, "https://upload.wikimedia.org/wikipedia/commons/thumb/8/81/Persialainen.jpg/200px-Persialainen.jpg");
            CreateAnimal(new Guid("f4faf111-e0ff-4581-a889-ec6f8095023a"), "Tom", "Ragdoll", 4, Color.White, "É bem ativo, gosta de pular das janelas, assim que cuidado se mora em apto", AnimalType.Cat, "https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Ragdoll_from_Gatil_Ragbelas.jpg/200px-Ragdoll_from_Gatil_Ragbelas.jpg");
            CreateAnimal(new Guid("077913e8-4548-40a2-9dd6-a205ab38687a"), "Rita", "Siamês", 5, Color.Gray, "Quietinha, bem tranquila e adora brincar com bolinhas", AnimalType.Cat, "http://image.cachorrogato.com.br/thumb/315/245/1/imagens/racas/imagem343.jpg");
            CreateAnimal(new Guid("d3ce789f-d637-4e50-9a13-44a373078de6"), "Raio", "Árabe", 5, Color.Brown, "Cavalho de trabalho pesado, gosta de corridas e é bem amistoso", AnimalType.Horse, "http://blog.brasilcowboy.com.br/wp-content/uploads/2016/03/cavalo-ra%C3%A7a-%C3%A1rabe.jpg");
        }

        private void AddAdopters(Guid userAdminId, Guid userAdopterId)
        {
            CreateAdopter(userAdminId, "Admin", "18992344312", "admin@teste.com");
            CreateAdopter(userAdopterId, "João", "18992344312", "js@gmail.com");
            CreateAdopter(new Guid("89bcbcd4-389b-4d1f-a959-57369aa9a238"), "Maria", "16992345651", "maa@yahoo.com.br");
            CreateAdopter(new Guid("1376be42-560a-4b23-adf8-8765f4ac76d4"), "Thiago", "11992309987", "th88@outlook.com.br");
        }

        private void AddAdoptions(Guid adopterId)
        {
            CreateAdoption(new Guid("5ef4c133-1cdd-4ef4-8726-22b285106a99"), adopterId, AdoptionStatus.Accepted, "Parabéns!! você tem uma nova mascota");
            CreateAdoption(new Guid("077913e8-4548-40a2-9dd6-a205ab38687a"), adopterId, AdoptionStatus.InProgress, "Estamos analisando seu pedido, em poucos dias damos uma resposta para você, fique ligado");
            CreateAdoption(new Guid("d3ce789f-d637-4e50-9a13-44a373078de6"), new Guid("89bcbcd4-389b-4d1f-a959-57369aa9a238"), AdoptionStatus.Rejected, "Achamos que você não tem condições para adotar um cavalho no seu apto");
            CreateAdoption(new Guid("cae8da45-db5b-4fc5-ad92-7bca814a8d5f"), new Guid("1376be42-560a-4b23-adf8-8765f4ac76d4"), AdoptionStatus.Requested, null);
        }

        private void CreateAdoption(Guid animalId, Guid adopterId, AdoptionStatus adoptionStatus, string observation)
        {
            if (!_context.Adoptions.Any(adoption => adoption.AnimalId.Equals(animalId) && adoption.AdopterId.Equals(adopterId) && adoption.AdoptionStatus.Equals(adoptionStatus)))
            {
                var adoption = new Adoption(animalId, adopterId);
                _context.Adoptions.Add(adoption);
                _context.SaveChanges();

                adoption.UpdateAdoptionStatus(adoptionStatus, observation);
                _context.SaveChanges();

                if (adoption.AdoptionStatus == AdoptionStatus.Accepted)
                {
                    var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);

                    if (animal != null)
                    {
                        animal.Adopt(adopterId);
                        _context.SaveChanges();
                    }
                }

                if(adoption.AdoptionStatus != AdoptionStatus.Rejected && adoption.AdoptionStatus != AdoptionStatus.Accepted)
                {
                    var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);

                    if (animal != null)
                    {
                        animal.InAdoptionProccess();
                        _context.SaveChanges();
                    }
                }
            }
        }

        private void CreateAnimal(Guid id, string name, string breed, int age, Color prevalentColor, string behavior, AnimalType animalType, string pictureUrl)
        {
            if (!_context.Animals.Any(animal => animal.Name.Equals(name)))
            {
                var animal = new Animal(id, name, breed, age, prevalentColor, behavior, animalType, pictureUrl);
                _context.Animals.Add(animal);
                _context.SaveChanges();
            }
        }

        private void CreateAdopter(Guid id, string name, string phoneNumber, string email)
        {
            if (!_context.Adopters.Any(adopter => adopter.Name.Equals(name)))
            {
                var adopter = new Adopter(id, name, phoneNumber, email);
                _context.Adopters.Add(adopter);
                _context.SaveChanges();
            }
        }
    }
}
