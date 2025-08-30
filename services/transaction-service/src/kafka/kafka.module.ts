import { forwardRef, Module } from '@nestjs/common';
import { KafkaService } from './services/kafka.service';
import { ConfigModule } from '@nestjs/config';
import { TransactionModule } from 'src/transaction/transaction.module';

@Module({
  imports: [ConfigModule, forwardRef(() => TransactionModule)],
  providers: [KafkaService],
  exports: [KafkaService],
})
export class KafkaModule {}
