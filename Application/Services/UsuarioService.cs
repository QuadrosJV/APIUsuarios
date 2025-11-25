using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using System.Collections.Generic;

namespace APIUsuarios.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public void CreateUsuario(UsuarioCreateDto usuarioCreateDto)
        {
            var usuario = new Usuario
            {
                Name = usuarioCreateDto.Name,
                Email = usuarioCreateDto.Email
            };

            _usuarioRepository.Add(usuario);
        }

        public UsuarioReadDto GetUsuario(int id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null) return null;

            return new UsuarioReadDto
            {
                Id = usuario.Id,
                Name = usuario.Name,
                Email = usuario.Email
            };
        }

        public void UpdateUsuario(UsuarioUpdateDto usuarioUpdateDto)
        {
            var usuario = _usuarioRepository.GetById(usuarioUpdateDto.Id);
            if (usuario == null) return;

            usuario.Name = usuarioUpdateDto.Name;
            usuario.Email = usuarioUpdateDto.Email;

            _usuarioRepository.Update(usuario);
        }

        public void DeleteUsuario(int id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null) return;

            _usuarioRepository.Delete(usuario);
        }

        public IEnumerable<UsuarioReadDto> GetAllUsuarios()
        {
            var usuarios = _usuarioRepository.GetAll();
            var usuarioReadDtos = new List<UsuarioReadDto>();

            foreach (var usuario in usuarios)
            {
                usuarioReadDtos.Add(new UsuarioReadDto
                {
                    Id = usuario.Id,
                    Name = usuario.Name,
                    Email = usuario.Email
                });
            }

            return usuarioReadDtos;
        }
    }
}