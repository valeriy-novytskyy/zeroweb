/*--------------------------------------------------------*\
|   ______    __   |
|  |  __  |  |  |  |
|  | |  | |  |  |  |
|  | !__! |  |  |  |
|  !______!  !__!  |  binary : tech art
|
|  @file Unit and functional test tasks.
|  @requires helpers
|----------------------------------------------------------
|  @author Valeriy Novytskyy
\*---------------------------------------------------------*/

'use strict';

var helpers = require('./helpers');

helpers.changeToProjectDir();

if (helpers.operation && helpers.operation.startsWith('f')) {
    helpers.run('protractor Scripts/tests/protractor.conf.js');
} else {
    helpers.run('karma start Scripts/tests/karma.conf.js');
}
