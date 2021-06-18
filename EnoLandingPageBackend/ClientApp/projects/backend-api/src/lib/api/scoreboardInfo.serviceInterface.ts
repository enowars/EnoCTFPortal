/**
 * EnoLandingPage
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */
import { HttpHeaders }                                       from '@angular/common/http';

import { Observable }                                        from 'rxjs';

import { Scoreboard } from '../model/models';


import { Configuration }                                     from '../configuration';



export interface ScoreboardInfoServiceInterface {
    defaultHeaders: HttpHeaders;
    configuration: Configuration;

    /**
     * Gets the current scoreboard.
     * 
     */
    apiScoreboardInfoScoreboardJsonGet(extraHttpRequestParams?: any): Observable<Scoreboard>;

    /**
     * Gets the scoreboard of a given roundId.
     * 
     * @param roundId Number of the round.
     */
    apiScoreboardInfoScoreboardroundIdJsonGet(roundId: number, extraHttpRequestParams?: any): Observable<Scoreboard>;

    /**
     * Gets the scoreboard of a given roundId.
     * 
     * @param roundId Number of the round.
     */
    apiScoreboardInfoScoreboardroundIdJsonPost(roundId: number, extraHttpRequestParams?: any): Observable<Scoreboard>;

}
