const english = {
"skip":"Skip to content","nav-about":"About","nav-experience":"Experience","nav-projects":"Projects","nav-contact":"Contact","header-contact":"Let's talk <span>↗</span>","whatsapp-label":"Chat on WhatsApp",
"hero-location":"BELO HORIZONTE, BRAZIL · 2026","hero-hello":"Hi, I'm","hero-role":"Software developer","hero-intro":"I work with application development, API integration and technical support. I like to understand the problem before I start writing code.","hero-see-projects":"Discuss an opportunity <span>↗</span>","hero-note":"A little code, curiosity and the desire to keep learning.","hero-current-label":"AREAS OF INTEREST","availability":"Open to opportunities","hero-bottom-left":"PERSONAL PORTFOLIO",
"about-label":"ABOUT ME","about-title":"I like technology for what it helps us <em>solve.</em>","about-p1":"I'm an IT technician graduated from Colégio Cotemig. I have practical experience in software development and level 2 technical support, working with application maintenance, APIs and problem analysis.","about-p2":"I want to keep growing as a developer, learning new tools and contributing to projects where I can make a real difference. To me, writing code also means understanding the context, testing ideas and paying attention to details.","fact-education-label":"EDUCATION","fact-education":"IT Technician · Cotemig","fact-location-label":"LOCATION","fact-focus-label":"FOCUS","fact-focus":"Software development",
"experience-label":"EXPERIENCE","experience-period":"PREVIOUS EXPERIENCE · INTERNSHIP","experience-role":"Systems Developer and Level 2 Support","experience-place":"Belo Horizonte, Brazil","experience-summary":"At Area Code, I got hands-on experience with software development and application support. I worked on feature maintenance, service integration and investigating day-to-day technical issues.","exp-1":"Maintained and developed features using C# and .NET.","exp-2":"Integrated and consumed REST APIs and handled JSON data.","exp-3":"Tested endpoints and checked responses with Swagger.","exp-4":"Worked with SQL queries and investigated data inconsistencies.","exp-5":"Provided Level 2 technical support and investigated incidents.",
"projects-label":"PROJECTS","projects-title":"A few things I've <em>built.</em>","projects-intro":"Study projects and practical exercises that are part of my journey.","project-pizza":"Pizzeria website","project-pizza-desc":"Academic website project for a pizzeria, focusing on page structure and content presentation.","project-gallery":"Photo website","project-gallery-desc":"An interface exercise to organize and display images on a web page.","project-periodic":"Periodic table","project-periodic-desc":"A reference page with information organized in a visual periodic table layout.","project-login":"Sign-up and login","project-login-desc":"Academic prototype with forms, DOM manipulation and local browser storage. It does not use server-side authentication.","view-code":"View code on GitHub","more-projects":"More code and experiments",
"skills-label":"TOOLS","skills-title":"Technologies I've worked <em>with.</em>","skills-development":"Development","skills-data":"Data and tools","skills-support":"Support and infrastructure","skills-support-list":"Level 2 support · Networks · Hardware · Cybersecurity","cert-label":"COURSES · CISCO NETWORKING ACADEMY",
"contact-label":"CONTACT","contact-title":"If you'd like to talk about technology or opportunities, <em>get in touch.</em>","contact-location":"Belo Horizonte, Minas Gerais · Brazil","footer-note":"Made by Augusto Mota · 2026","back-top":"Back to top ↑"
};
const button=document.getElementById("language-toggle");
let currentLanguage="pt";
const translatable=[...document.querySelectorAll("[data-i18n]")];
translatable.forEach(el=>{el.dataset.pt=el.innerHTML;});
function setLanguage(language){
 currentLanguage=language;
 document.documentElement.lang=language==="en"?"en":"pt-BR";
 translatable.forEach(el=>{const key=el.dataset.i18n;el.innerHTML=language==="en"&&english[key]?english[key]:el.dataset.pt;});
 button.textContent=language==="en"?"PT":"EN";
 button.setAttribute("aria-label",language==="en"?"Mudar idioma para português":"Switch language to English");
}
button.addEventListener("click",()=>setLanguage(currentLanguage==="pt"?"en":"pt"));
const menuButton=document.getElementById("menu-toggle");
const nav=document.getElementById("navigation");
menuButton.addEventListener("click",()=>{const open=menuButton.getAttribute("aria-expanded")==="true";menuButton.setAttribute("aria-expanded",String(!open));nav.classList.toggle("open",!open);});
nav.querySelectorAll("a").forEach(link=>link.addEventListener("click",()=>{nav.classList.remove("open");menuButton.setAttribute("aria-expanded","false");}));
if("IntersectionObserver"in window&&!window.matchMedia("(prefers-reduced-motion: reduce)").matches){
 const observer=new IntersectionObserver(entries=>entries.forEach(entry=>{if(entry.isIntersecting){entry.target.classList.add("visible");observer.unobserve(entry.target);}}),{threshold:.12});
 document.querySelectorAll(".section-block,.project-row,.hero-main,.hero-aside").forEach(el=>{el.classList.add("reveal");observer.observe(el);});
}
