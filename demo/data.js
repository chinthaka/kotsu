/*
 * Sample content for the Kotsu demo: launch countries, stages, boards and subjects.
 * Each sub-topic is [name, estimated study hours].
 */
window.KOTSU_DATA = {
  countries: [
    {
      id: 'england',
      name: 'England',
      badge: 'EN',
      colour: '#e11d48',
      stages: [
        {
          id: 'england-ks4',
          name: 'Key Stage 4',
          hint: 'GCSEs, usually Years 10 and 11',
          subjects: ['maths-gcse', 'biology-gcse', 'chemistry', 'english-lit', 'history', 'geography'],
        },
        {
          id: 'england-ks5',
          name: 'Key Stage 5',
          hint: 'A levels, usually Years 12 and 13',
          subjects: ['maths-advanced', 'biology-advanced', 'chemistry', 'psychology', 'economics'],
        },
      ],
      boards: [
        { id: 'aqa', name: 'AQA' },
        { id: 'pearson-edexcel', name: 'Pearson Edexcel' },
        { id: 'ocr', name: 'OCR' },
      ],
    },
    {
      id: 'wales',
      name: 'Wales',
      badge: 'CY',
      colour: '#16a34a',
      stages: [
        {
          id: 'wales-ks4-gcse',
          name: 'Key Stage 4 (GCSE)',
          hint: 'GCSEs, usually Years 10 and 11',
          subjects: ['maths-gcse', 'biology-gcse', 'english-lit', 'geography', 'history'],
        },
        {
          id: 'wales-post-16',
          name: 'Post-16 (AS and A level)',
          hint: 'AS and A levels, usually Years 12 and 13',
          subjects: ['maths-advanced', 'biology-advanced', 'psychology', 'economics'],
        },
      ],
      boards: [{ id: 'wjec', name: 'WJEC' }],
    },
    {
      id: 'scotland',
      name: 'Scotland',
      badge: 'SC',
      colour: '#2563eb',
      stages: [
        {
          id: 'sqa-national-5',
          name: 'National 5',
          hint: 'Usually S4',
          subjects: ['maths-gcse', 'chemistry', 'physics', 'english', 'history'],
        },
        {
          id: 'sqa-higher',
          name: 'Higher',
          hint: 'Usually S5',
          subjects: ['maths-advanced', 'physics', 'chemistry', 'english', 'geography'],
        },
        {
          id: 'sqa-advanced-higher',
          name: 'Advanced Higher',
          hint: 'Usually S6',
          subjects: ['maths-advanced', 'biology-advanced', 'physics'],
        },
      ],
      boards: [{ id: 'sqa', name: 'SQA' }],
    },
  ],

  subjects: {
    'maths-gcse': {
      name: 'Maths',
      icon: '➗',
      topics: [
        { name: 'Number', subs: [['Fractions, decimals and percentages', 1.5], ['Powers and roots', 1], ['Ratio and proportion', 1.5]] },
        { name: 'Algebra', subs: [['Expressions and equations', 2], ['Straight-line graphs', 1.5], ['Sequences', 1]] },
        { name: 'Geometry', subs: [['Angles and shapes', 1], ['Pythagoras and trigonometry', 2], ['Area and volume', 1.5]] },
        { name: 'Statistics', subs: [['Probability', 1.5], ['Charts and averages', 1]] },
      ],
    },
    'maths-advanced': {
      name: 'Maths',
      icon: '∫',
      topics: [
        { name: 'Pure maths', subs: [['Algebra and functions', 2], ['Differentiation and integration', 3], ['Trigonometry', 2], ['Exponentials and logarithms', 1.5]] },
        { name: 'Statistics', subs: [['Probability distributions', 2], ['Hypothesis testing', 1.5]] },
        { name: 'Mechanics', subs: [['Kinematics', 1.5], ["Forces and Newton's laws", 2]] },
      ],
    },
    'biology-gcse': {
      name: 'Biology',
      icon: '🧬',
      topics: [
        { name: 'Cells', subs: [['Cell structure', 1], ['Cell division', 1], ['Transport in cells', 1.5]] },
        { name: 'Organisation', subs: [['The digestive system', 1], ['The heart and blood', 1.5], ['Plant tissues', 1]] },
        { name: 'Infection and response', subs: [['Diseases', 1], ['The immune system', 1.5]] },
        { name: 'Ecology', subs: [['Food chains', 1], ['Biodiversity', 1]] },
      ],
    },
    'biology-advanced': {
      name: 'Biology',
      icon: '🧬',
      topics: [
        { name: 'Biological molecules', subs: [['Proteins and enzymes', 2], ['DNA and replication', 1.5]] },
        { name: 'Cells', subs: [['Cell membranes', 1.5], ['The immune system', 1.5]] },
        { name: 'Genetics', subs: [['Inheritance', 2], ['Evolution', 1.5]] },
      ],
    },
    chemistry: {
      name: 'Chemistry',
      icon: '⚗️',
      topics: [
        { name: 'Atomic structure', subs: [['Atoms and isotopes', 1], ['The periodic table', 1.5]] },
        { name: 'Bonding', subs: [['Ionic and covalent bonding', 1.5], ['Structure and properties', 1]] },
        { name: 'Reactions', subs: [['Rates of reaction', 1.5], ['Energy changes', 1], ['Equilibrium', 1.5]] },
        { name: 'Organic chemistry', subs: [['Hydrocarbons', 1], ['Functional groups', 1.5]] },
      ],
    },
    physics: {
      name: 'Physics',
      icon: '🧲',
      topics: [
        { name: 'Mechanics', subs: [['Motion', 1.5], ['Forces', 1.5], ['Energy and power', 1]] },
        { name: 'Electricity', subs: [['Circuits', 1.5], ['Resistance', 1]] },
        { name: 'Waves', subs: [['Wave properties', 1], ['Light', 1]] },
        { name: 'Radiation', subs: [['Nuclear decay', 1.5], ['Half-life', 1]] },
      ],
    },
    'english-lit': {
      name: 'English Literature',
      icon: '📖',
      topics: [
        { name: 'Shakespeare', subs: [['Plot and characters', 1.5], ['Themes', 1], ['Key quotations', 1]] },
        { name: '19th-century novel', subs: [['Plot and characters', 1.5], ['Context', 1]] },
        { name: 'Modern text', subs: [['Plot and characters', 1], ['Themes', 1]] },
        { name: 'Poetry', subs: [['Anthology poems', 2], ['Unseen poetry', 1]] },
      ],
    },
    english: {
      name: 'English',
      icon: '✍️',
      topics: [
        { name: 'Reading', subs: [['Close reading', 1.5], ['Comparing texts', 1]] },
        { name: 'Writing', subs: [['Descriptive writing', 1.5], ['Persuasive writing', 1.5]] },
        { name: 'Critical essay', subs: [['Essay structure', 1], ['Set text', 2]] },
      ],
    },
    history: {
      name: 'History',
      icon: '🏰',
      topics: [
        { name: 'Conflict and tension', subs: [['Causes of the First World War', 1.5], ['The peace settlement', 1]] },
        { name: 'Health and the people', subs: [['Medieval medicine', 1], ['Modern medicine', 1.5]] },
        { name: 'Elizabethan England', subs: [['Court and parliament', 1], ['Daily life', 1], ['Troubles at home and abroad', 1]] },
      ],
    },
    geography: {
      name: 'Geography',
      icon: '🌍',
      topics: [
        { name: 'Natural hazards', subs: [['Tectonic hazards', 1.5], ['Weather hazards', 1], ['Climate change', 1.5]] },
        { name: 'The living world', subs: [['Ecosystems', 1], ['Tropical rainforests', 1]] },
        { name: 'Urban issues', subs: [['Cities around the world', 1.5], ['Sustainable living', 1]] },
      ],
    },
    psychology: {
      name: 'Psychology',
      icon: '🧠',
      topics: [
        { name: 'Memory', subs: [['Models of memory', 1.5], ['Eyewitness testimony', 1]] },
        { name: 'Attachment', subs: [['Theories of attachment', 1.5], ['Caregiver interactions', 1]] },
        { name: 'Research methods', subs: [['Experiments', 1.5], ['Handling data', 1.5]] },
        { name: 'Psychopathology', subs: [['Phobias and OCD', 1], ['Depression', 1]] },
      ],
    },
    economics: {
      name: 'Economics',
      icon: '📈',
      topics: [
        { name: 'Markets', subs: [['Supply and demand', 1.5], ['Elasticity', 1.5], ['Market failure', 1.5]] },
        { name: 'The national economy', subs: [['Inflation', 1], ['Unemployment', 1], ['Fiscal and monetary policy', 2]] },
      ],
    },
  },
};
