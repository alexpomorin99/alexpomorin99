// Telegram bot + Firebase Firestore for leads
const { Telegraf } = require('telegraf');
const { initializeApp, applicationDefault } = require('firebase-admin/app');
const { getFirestore } = require('firebase-admin/firestore');

initializeApp({
  credential: applicationDefault(), // or use service account JSON
});

const db = getFirestore();
const bot = new Telegraf(process.env.TG_BOT_TOKEN || 'YOUR_TOKEN_HERE');

bot.start(async (ctx) => {
  await ctx.reply('Привет! Я бот-лидогенератор. Напиши свое имя и чем интересуешься.');
});

bot.on('text', async (ctx) => {
  const text = ctx.message.text;
  const userId = String(ctx.from.id);
  await db.collection('leads').add({
    userId,
    text,
    createdAt: new Date().toISOString(),
  });
  await ctx.reply('Спасибо! Я сохранил твой запрос, с вами свяжется менеджер.');
});

bot.launch();
console.log('Telegram bot with Firebase is running');
