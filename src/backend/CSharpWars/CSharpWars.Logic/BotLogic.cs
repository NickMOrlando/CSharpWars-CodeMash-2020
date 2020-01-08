using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CSharpWars.Common.Configuration.Interfaces;
using CSharpWars.Common.Extensions;
using CSharpWars.Common.Helpers.Interfaces;
using CSharpWars.DataAccess.Repositories.Interfaces;
using CSharpWars.DtoModel;
using CSharpWars.Enums;
using CSharpWars.Logic.Interfaces;
using CSharpWars.Mapping.Interfaces;
using CSharpWars.Model;

namespace CSharpWars.Logic
{
    public class BotLogic : IBotLogic
    {
        private readonly IRandomHelper _randomHelper;
        private readonly IRepository<Bot> _botRepository;
        private readonly IRepository<BotScript> _scriptRepository;
        private readonly IRepository<Bot, BotScript> _botScriptRepository;
        private readonly IRepository<Player> _playerRepository;
        private readonly IMapper<Bot, BotDto> _botMapper;
        private readonly IMapper<Bot, BotToCreateDto> _botToCreateMapper;
        private readonly IArenaLogic _arenaLogic;
        private Random _random = new Random();

        public BotLogic(
            IRandomHelper randomHelper,
            IRepository<Bot> botRepository,
            IRepository<BotScript> scriptRepository,
            IRepository<Bot, BotScript> botScriptRepository,
            IRepository<Player> playerRepository,
            IMapper<Bot, BotDto> botMapper,
            IMapper<Bot, BotToCreateDto> botToCreateMapper,
            IArenaLogic arenaLogic)
        {
            _randomHelper = randomHelper;
            _botRepository = botRepository;
            _scriptRepository = scriptRepository;
            _botScriptRepository = botScriptRepository;
            _playerRepository = playerRepository;
            _botMapper = botMapper;
            _botToCreateMapper = botToCreateMapper;
            _arenaLogic = arenaLogic;
        }

        public async Task<IList<BotDto>> GetAllActiveBots()
        {
            var dateTimeToCompare = DateTime.UtcNow.AddSeconds(-10);
            var activeBots = await _botRepository.Find(
                x => x.CurrentHealth > 0 || x.TimeOfDeath > dateTimeToCompare, 
                include => include.Player);
            return _botMapper.Map(activeBots);
        }

        public async Task<IList<BotDto>> GetAllLiveBots()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetBotScript(Guid botId)
        {
            throw new NotImplementedException();
        }

        public async Task<BotDto> CreateBot(BotToCreateDto botToCreate)
        {
            var bot = _botToCreateMapper.Map(botToCreate);
            var arena = await _arenaLogic.GetArena();
            var player = await _playerRepository.Single(x => x.Id == botToCreate.PlayerId);
            player.LastDeployment = DateTime.UtcNow;
            
            bot.Player = player;
            bot.Orientation = _randomHelper.Get<PossibleOrientations>();
            bot.X = _random.Next(-arena.Height, arena.Height) / 2;
            bot.Y = _random.Next(-arena.Width, arena.Width) / 2;
            bot.CurrentHealth = bot.MaximumHealth;
            bot.CurrentStamina = bot.MaximumStamina;
            bot.Memory = new Dictionary<string, string>().Serialize();

            bot.TimeOfDeath = DateTime.MaxValue;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bot = await _botRepository.Create(bot);
                var botScript = await _scriptRepository.Single(x => x.Id == bot.Id);
                botScript.Script = botToCreate.Script;
                await _scriptRepository.Update(botScript);
                await _playerRepository.Update(player);
                transaction.Complete();
            }

            var createdBot = _botMapper.Map(bot);
            return createdBot;
        }

        public async Task UpdateBots(IList<BotDto> bots)
        {
            throw new NotImplementedException();
        }
    }
}