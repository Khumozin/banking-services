import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { Response, Request } from 'express';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  // CORS
  app.enableCors();

  // Health check endpoint
  app.getHttpAdapter().get('/health', (req: Request, res: Response) => {
    res.json({
      status: 'healthy',
      service: 'notification-service',
      timestamp: new Date().toISOString(),
    });
  });

  const PORT = process.env.PORT ?? 3000;
  await app.listen(PORT);
  console.log(`Notification Service is running on http://localhost:${PORT}`);
}
bootstrap();
