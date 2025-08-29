import { Injectable, Logger, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { KafkaService } from 'src/kafka/services/kafka.service';
import { Repository } from 'typeorm';
import { Transaction } from '../entities/transaction.entity';
import {
  DepositDto,
  TransactionInitiatedEvent,
  TransferDto,
} from '../dtos/transaction.dto';

@Injectable()
export class TransactionService {
  private readonly logger = new Logger(TransactionService.name);

  constructor(
    @InjectRepository(Transaction)
    private readonly transactionRepository: Repository<Transaction>,
    private readonly kafkaService: KafkaService,
  ) {}

  async createDeposit(depositDto: DepositDto): Promise<Transaction> {
    this.logger.log(
      `Creating deposit transaction for account ${depositDto.destinationAccountId}`,
    );

    // Create transaction record
    const transaction = this.transactionRepository.create({
      destinationAccountId: depositDto.destinationAccountId,
      amount: depositDto.amount,
      status: 'PENDING',
      transactionType: 'DEPOSIT',
    });

    const savedTransaction = await this.transactionRepository.save(transaction);

    // Publish transaction.initiated event
    const event: TransactionInitiatedEvent = {
      transactionId: savedTransaction.transactionId,
      destinationAccountId: savedTransaction.destinationAccountId,
      amount: savedTransaction.amount,
      transactionType: savedTransaction.transactionType,
    };

    await this.kafkaService.publish('transaction.initiated', event);

    this.logger.log(
      `Deposit transaction created: ${savedTransaction.transactionId}`,
    );
    return savedTransaction;
  }

  async createTransfer(transferDto: TransferDto): Promise<Transaction> {
    this.logger.log(
      `Creating transfer transaction from ${transferDto.sourceAccountId} to ${transferDto.destinationAccountId}`,
    );

    // Create transaction record
    const transaction = this.transactionRepository.create({
      sourceAccountId: transferDto.sourceAccountId,
      destinationAccountId: transferDto.destinationAccountId,
      amount: transferDto.amount,
      status: 'PENDING',
      transactionType: 'TRANSFER',
    });

    const savedTransaction = await this.transactionRepository.save(transaction);

    // Publish transaction.initiated event
    const event: TransactionInitiatedEvent = {
      transactionId: savedTransaction.transactionId,
      sourceAccountId: savedTransaction.sourceAccountId,
      destinationAccountId: savedTransaction.destinationAccountId,
      amount: savedTransaction.amount,
      transactionType: savedTransaction.transactionType,
    };

    await this.kafkaService.publish('transaction.initiated', event);

    this.logger.log(
      `Transfer transaction created: ${savedTransaction.transactionId}`,
    );
    return savedTransaction;
  }

  async getTransaction(transactionId: string): Promise<Transaction> {
    const transaction = await this.transactionRepository.findOne({
      where: { transactionId },
    });

    if (!transaction) {
      throw new NotFoundException(
        `Transaction with ID ${transactionId} not found`,
      );
    }

    return transaction;
  }

  async getAllTransactions(): Promise<Transaction[]> {
    return this.transactionRepository.find({
      order: { createdAt: 'DESC' },
    });
  }
}
