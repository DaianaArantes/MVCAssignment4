/*
* KGSail MVC Application Assignment 4
*
* KGMemberController process incoming requests,
* handle user input and interactions, and execute appropriate application logic.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KGSail.Models;

namespace KGSail.Controllers
{
    public class KGMemberController : Controller
    {
        private readonly SailContext _context;

        public KGMemberController(SailContext context)
        {
            _context = context;
        }

        // GET: KGMember
        // Return view with members ordered by fullName
        public async Task<IActionResult> Index()
        {

            return View(await _context.Member
                .OrderBy(e => e.FullName)
                .ToListAsync());

        }

        // GET: KGMember
        // Return view with details about selected member
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.ProvinceCodeNavigation)
                .SingleOrDefaultAsync(m => m.MemberId == id);

            ViewData["Title"] = "Details of " + member.FullName;

	    if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: KGMember/Create
        // Return view with form to create new member
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: KGMember
        // If no errors, save new member to database
        // Return to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,FullName,FirstName,LastName,SpouseFirstName,SpouseLastName,Street,City,ProvinceCode,PostalCode,HomePhone,Email,YearJoined,Comment,TaskExempt,UseCanadaPost")] Member member)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Member " + member.FullName + " was created";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", $"error inserting new member: {ex.GetBaseException().Message}");

            }


            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", member.ProvinceCode);
            return View(member);
        }

        // GET: KGMember
        // Return view if member information to edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var getMember = await _context.Member.SingleOrDefaultAsync(m => m.MemberId == id);
            
            if (getMember == null)
            {
                return NotFound();
            }

            var provName = _context.Province
                .OrderBy(a=> a.Name);

            

            ViewData["Title"] = "Edit  " + getMember.FullName;
            ViewData["ProvinceCode"] = new SelectList(provName, "ProvinceCode", "Name");
            return View(getMember);
        }

        // POST: KGMember
        // If no errors, save editions to database
        // Return to index view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,FullName,FirstName,LastName,SpouseFirstName,SpouseLastName,Street,City,ProvinceCode,PostalCode,HomePhone,Email,YearJoined,Comment,TaskExempt,UseCanadaPost")] Member member)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Member " + member.FullName + " was edited";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("Eerror encountered while editing: ", ex.GetBaseException().Message);
            }
            


            ViewData["Title"] = "Edit  " + member.FullName;
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(a=> a.Name), "ProvinceCode", "Name");
            return View(member);
        }

        // GET: KGMember
        // Show information about selected member before deleting
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.ProvinceCodeNavigation)
                .SingleOrDefaultAsync(m => m.MemberId == id);
         

            ViewData["Title"] = "Delete " + member.FullName;

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: KGMember
        // Delete member from database
        // Return to index view
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var member = await _context.Member.SingleOrDefaultAsync(m => m.MemberId == id);
                _context.Member.Remove(member);
                await _context.SaveChangesAsync();
                TempData["message"] = "Member " + member.FullName + " was deleted";
                return RedirectToAction(nameof(Index));
            }
            // Innermost exception message
            catch (Exception ex)
            {
                TempData["Message"] = ex.GetBaseException().ToString();
                return View();
            }
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }
    }
}
