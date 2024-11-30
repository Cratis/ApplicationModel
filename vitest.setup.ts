import 'reflect-metadata';
import * as chai from 'chai';
chai.should();
import * as chaiAsPromised from 'chai-as-promised';
import * as sinonChai from 'sinon-chai';
chai.use(sinonChai.default);
chai.use(chaiAsPromised.default);
