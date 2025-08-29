import { Body, Controller, Get, Param, Post, Query } from '@nestjs/common';
import { TransactionService } from '../services/transaction.service';
import { Transaction } from '../entities/transaction.entity';
import { ApiOperation, ApiResponse, ApiQuery, ApiParam } from '@nestjs/swagger';
import {
  TransactionResponseDto,
  DepositDto,
  TransferDto,
} from '../dtos/transaction.dto';

@Controller('api/transactions')
export class TransactionController {
  constructor(private readonly transactionService: TransactionService) {}
  @Post('deposit')
  @ApiOperation({ summary: 'Create a deposit transaction' })
  @ApiResponse({
    status: 201,
    description: 'Deposit transaction created successfully',
    type: TransactionResponseDto,
  })
  @ApiResponse({ status: 400, description: 'Bad request' })
  async createDeposit(
    @Body() depositDto: DepositDto,
  ): Promise<TransactionResponseDto> {
    const transaction = await this.transactionService.createDeposit(depositDto);
    return this.mapToResponseDto(transaction);
  }

  @Post('transfer')
  @ApiOperation({ summary: 'Create a transfer transaction' })
  @ApiResponse({
    status: 201,
    description: 'Transfer transaction created successfully',
    type: TransactionResponseDto,
  })
  @ApiResponse({ status: 400, description: 'Bad request' })
  async createTransfer(
    @Body() transferDto: TransferDto,
  ): Promise<TransactionResponseDto> {
    const transaction =
      await this.transactionService.createTransfer(transferDto);
    return this.mapToResponseDto(transaction);
  }

  @Get()
  @ApiOperation({ summary: 'Get all transactions' })
  @ApiQuery({
    name: 'status',
    required: false,
    description: 'Filter by transaction status',
  })
  @ApiResponse({
    status: 200,
    description: 'List of transactions',
    type: [TransactionResponseDto],
  })
  async getAllTransactions(
    @Query('status') status?: string,
  ): Promise<TransactionResponseDto[]> {
    let transactions: Transaction[];

    if (status) {
      transactions =
        await this.transactionService.getTransactionsByStatus(status);
    } else {
      transactions = await this.transactionService.getAllTransactions();
    }

    return transactions.map((transaction) =>
      this.mapToResponseDto(transaction),
    );
  }

  @Get(':transactionId')
  @ApiOperation({ summary: 'Get transaction by ID' })
  @ApiParam({ name: 'transactionId', description: 'Transaction ID' })
  @ApiResponse({
    status: 200,
    description: 'Transaction found',
    type: TransactionResponseDto,
  })
  @ApiResponse({ status: 404, description: 'Transaction not found' })
  async getTransaction(
    @Param('transactionId') transactionId: string,
  ): Promise<TransactionResponseDto> {
    const transaction =
      await this.transactionService.getTransaction(transactionId);
    return this.mapToResponseDto(transaction);
  }

  @Get('account/:accountId')
  @ApiOperation({ summary: 'Get transactions for a specific account' })
  @ApiParam({ name: 'accountId', description: 'Account ID' })
  @ApiResponse({
    status: 200,
    description: 'List of transactions for the account',
    type: [TransactionResponseDto],
  })
  async getTransactionsByAccount(
    @Param('accountId') accountId: string,
  ): Promise<TransactionResponseDto[]> {
    const transactions =
      await this.transactionService.getTransactionsByAccount(accountId);
    return transactions.map((transaction) =>
      this.mapToResponseDto(transaction),
    );
  }

  private mapToResponseDto(transaction: Transaction): TransactionResponseDto {
    return {
      transactionId: transaction.transactionId,
      sourceAccountId: transaction.sourceAccountId,
      destinationAccountId: transaction.destinationAccountId,
      amount: transaction.amount,
      status: transaction.status,
      transactionType: transaction.transactionType,
      createdAt: transaction.createdAt,
      updatedAt: transaction.updatedAt,
    };
  }
}
