// src/app/shared/components/global-loading/global-loading.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';

@Component({
    selector: 'app-global-loading',
    standalone: true,
    imports: [CommonModule, LoadingSpinnerComponent],
    template: `
        @if (loading$ | async; as loadingState) {
            @if (loadingState.loading) {
                <app-loading-spinner
                    [overlay]="true"
                    [message]="loadingState.message ?? ''"
                    [size]='48'>
                </app-loading-spinner>
            }
        }
    `
})
export class GlobalLoadingComponent {
    loading$;

    constructor(private readonly loadingService: LoadingService) {
        this.loading$ = this.loadingService.loading$;
    }
}