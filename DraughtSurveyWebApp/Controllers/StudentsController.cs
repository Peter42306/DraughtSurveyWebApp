using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Interfaces;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.ViewModels;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IRepository<Student> _repository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(
            IRepository<Student> repository, 
            IImageService imageService, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager
            )            

        {
            _repository = repository;
            _imageService = imageService;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: Students        
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var allStudents = await _repository.GetAllAsync();

            if (!User.IsInRole("Admin"))
            {
                allStudents = allStudents.Where(s => s.UserId == currentUser.Id).ToList();
            }

            return View(allStudents);
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        // GET: Students/Create
        public IActionResult Create()
        {
            return View(new StudentViewModel());
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var student = _mapper.Map<Student>(viewModel);

            if(viewModel.Photo != null)
            {
                var photoPath = await _imageService.SaveImageAsync(viewModel.Photo);
                student.PhotoPath = photoPath;
            }

            var currentUser = await _userManager.GetUserAsync(User);
            student.UserId = currentUser.Id;

            await _repository.AddAsync(student);
            await _repository.SaveAsync();

            return RedirectToAction(nameof(Index));            
        }        

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _repository.GetByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && student.UserId !=currentUser.Id)
            {
                return Forbid();
            }

            var viewModel = _mapper.Map<StudentViewModel>(student);
            return View(viewModel);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var studentToUpdate = await _repository.GetByIdAsync(viewModel.Id.Value);
            if (studentToUpdate == null)
            {                
                return NotFound();
            }

            // Update model fields from the ViewModel
            _mapper.Map(viewModel, studentToUpdate);

            // Update photo if a new photo is uploaded
            if (viewModel.Photo!= null)
            {
                // Delete the old photo
                _imageService.DeleteImage(studentToUpdate.PhotoPath);

                // Save the new one
                var newPhotoPath = await _imageService.SaveImageAsync(viewModel.Photo);
                studentToUpdate.PhotoPath = newPhotoPath;
            }

            _repository.Update(studentToUpdate);
            await _repository.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _repository.GetByIdAsync(id.Value);                                
            if (student == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if(!User.IsInRole("Admin") && student.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            var currentUser = await _userManager.GetUserAsync(User);

            if (student == null)
            {
                return NotFound();
            }
            else
            {
                if (!User.IsInRole("Admin") && student.UserId != currentUser.Id)
                {
                    return Forbid();
                }

                // Delete the photo before deleting
                _imageService.DeleteImage(student.PhotoPath);

                _repository.Delete(student);
                await _repository.SaveAsync();
            }            
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _repository.GetByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if(!User.IsInRole("Admin") && student.UserId !=currentUser.Id)
            {
                return Forbid();
            }

            return View(student);
        }        
    }
}
