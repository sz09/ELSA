import { NgModule } from '@angular/core';
import { RouterModule, Routes, UrlMatcher, UrlSegment } from '@angular/router';
import { NotFoundComponent } from './not-found/not-found.component';
import { PublicLayoutComponent } from './public/public-layout/public-layout.component';
import { LoginComponent } from './login/login.component';
const redirectToHomeMatcher: UrlMatcher = (segments: UrlSegment[]) => {
  if (segments.length === 0) {
    return {
      consumed: segments
    };
  }
  return null;
};

const redirectToAdminDefaultRoutMatcher: UrlMatcher = (segments: UrlSegment[]) => {
  if (segments.length === 1 && segments[0].path === 'admin') {
    return {
      consumed: []
    };
  }
  return null;
};

const routes: Routes = [
  {
    matcher: redirectToHomeMatcher,
    redirectTo: 'home',
    pathMatch: 'full' 
  },
  {
    matcher: redirectToAdminDefaultRoutMatcher,
    redirectTo: '/admin/quizzes',
    pathMatch: 'full' 
  },
  { 
    path: 'login', 
    component: LoginComponent 
  },
  {
    path: 'admin', loadChildren: async () => {
      const m = await import('./admin/admin.routing.module');
      return m.AdminRoutingModule;
    }
  },
  {
    path: '', loadChildren: async () => {
      const m = await import('./public/public.routing.module');
      return m.PublicRoutingModule;
    }
  },
  { path: '**', component: NotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
